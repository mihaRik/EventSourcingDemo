using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.PubSub;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace EventSourcingDemo.EventStore.Repository
{
    public class EventStoreRepository<TAggregate> : IEventStoreRepository<TAggregate>
        where TAggregate : IAggregate
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public EventStoreRepository(IEventStore eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<TAggregate> GetByIdAsync(Guid id)
        {
            var aggregate = CreateEmptyAggregate();

            foreach (var @event in await _eventStore.ReadEventsAsync(id))
            {
                ((dynamic)aggregate).ApplyEvent((dynamic)@event, @event.AggregateVersion);
            }

            if (aggregate.IsDeleted)
            {
                throw new KeyNotFoundException($"There is no aggregate with id: { id }");
            }

            return aggregate;
        }

        public async Task SaveAsync(IAggregate aggregate)
        {
            foreach (var @event in aggregate.GetUncommittedEvents())
            {
                await _eventStore.AppendEventAsync(@event);
                await _eventPublisher.PublishAsync((dynamic)@event);
            }

            aggregate.ClearUncommittedEvents();
        }

        private TAggregate CreateEmptyAggregate()
        {
            return (TAggregate)typeof(TAggregate).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                                                 null, new Type[0], new ParameterModifier[0])
                                                 .Invoke(new object[0]);
        }
    }
}
