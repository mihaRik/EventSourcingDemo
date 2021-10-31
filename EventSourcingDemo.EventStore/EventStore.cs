using EventSourcingDemo.Domain.Base;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;

        public EventStore(IEventStoreConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task AppendEventAsync(IEvent @event)
        {
            var eventData = new EventData(@event.EventId, @event.GetType().AssemblyQualifiedName, true, Serialize(@event), Encoding.UTF8.GetBytes("{}"));

            var aggregateVersion = @event.AggregateVersion == -1 ? ExpectedVersion.NoStream : @event.AggregateVersion - 1;

            await _connection.AppendToStreamAsync(@event.AggregateId.ToString(), aggregateVersion, eventData);
        }

        public async Task<IEnumerable<IEvent>> ReadEventsAsync(Guid aggregateId)
        {
            var result = new List<IEvent>();
            long nextSliceStart = StreamPosition.Start;
            StreamEventsSlice currentSlice;

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(aggregateId.ToString(), nextSliceStart, 10, false);

                if (currentSlice.Status != SliceReadStatus.Success)
                {
                    throw new InvalidOperationException($"Aggregate { aggregateId } not found");
                }

                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var  @event in currentSlice.Events)
                {
                    var type = Type.GetType(@event.Event.EventType);
                    var data = Encoding.UTF8.GetString(@event.Event.Data);

                    result.Add((IEvent)JsonConvert.DeserializeObject(data, type));
                }

            } while (!currentSlice.IsEndOfStream);
            
            return result;
        }

        private static byte[] Serialize(IEvent @event)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
        }
    }
}
