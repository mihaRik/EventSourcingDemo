using System;

namespace EventSourcingDemo.Domain.Base
{
    public interface IEvent
    {
        Guid EventId {  get; }

        Guid AggregateId { get; set; }

        long AggregateVersion { get; set; }
    }
}
