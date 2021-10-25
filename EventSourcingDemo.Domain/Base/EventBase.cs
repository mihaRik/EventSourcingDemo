using System;

namespace EventSourcingDemo.Domain.Base
{
    public class EventBase : IEvent
    {
        public Guid EventId { get; private set; }

        public Guid AggregateId { get; set; }

        public long AggregateVersion { get; set; }

        public EventBase()
        {
            EventId = Guid.NewGuid();
        }
    }
}
