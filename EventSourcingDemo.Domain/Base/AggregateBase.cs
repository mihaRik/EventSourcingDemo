using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcingDemo.Domain.Base
{
    public class AggregateBase : IAggregate
    {
        public Guid AggregateId { get; protected set; }

        public long AggregateVersion { get; protected set; } = -1;

        public bool IsDeleted {  get; protected set; }

        private readonly ICollection<IEvent> _uncommitedEvents = new List<IEvent>();

        public void ApplyEvent(IEvent @event, long version)
        {
            if (!_uncommitedEvents.Any(_ => _.EventId == @event.EventId))
            {
                AggregateVersion = version;

                ((dynamic)this).Apply((dynamic)@event);
            }
        }

        public void RaiseEvent(IEvent @event)
        {
            AggregateVersion++;

            @event.AggregateId = AggregateId;
            @event.AggregateVersion = AggregateVersion;

            ApplyEvent(@event, AggregateVersion);

            _uncommitedEvents.Add(@event);
        }

        public IEnumerable<IEvent> GetUncommittedEvents()
        {
            return _uncommitedEvents;
        }

        public void ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }
    }
}
