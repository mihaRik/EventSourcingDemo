using EventSourcingDemo.Domain.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcingDemo.EventStore
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> ReadEventsAsync(Guid aggregateId);
        Task AppendEventAsync(IEvent @event);
    }
}
