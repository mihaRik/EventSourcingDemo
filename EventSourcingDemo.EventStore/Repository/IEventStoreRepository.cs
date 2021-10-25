using EventSourcingDemo.Domain.Base;
using System;
using System.Threading.Tasks;

namespace EventSourcingDemo.EventStore.Repository
{
    public interface IEventStoreRepository<TAggregate> where TAggregate : IAggregate
    {
        Task<TAggregate> GetByIdAsync(Guid id);
        Task SaveAsync(IAggregate aggregate);
    }
}
