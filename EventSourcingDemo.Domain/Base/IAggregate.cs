using System;
using System.Collections.Generic;

namespace EventSourcingDemo.Domain.Base
{
    public interface IAggregate
    {
        Guid AggregateId { get; }

        long AggregateVersion {  get; }

        bool IsDeleted { get; }

        void ApplyEvent(IEvent @event, long version);
        IEnumerable<IEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}