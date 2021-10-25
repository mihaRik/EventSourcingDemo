using EventSourcingDemo.Domain.Warehouse;
using EventSourcingDemo.EventStore.Repository;
using EventSourcingDemo.ReadModel;
using EventSourcingDemo.ReadModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Warehouse
{
    public class WarehouseReader : IWarehouseReader
    {
        private readonly IReadRepository<WarehouseReadModel> _readRepository;
        private readonly IReadRepository<WarehouseItemReadModel> _itemReadRepository;
        private readonly IEventStoreRepository<WarehouseAggregate> _eventStoreRepository;

        public WarehouseReader(IReadRepository<WarehouseReadModel> readRepository,
                               IEventStoreRepository<WarehouseAggregate> eventStoreRepository,
                               IReadRepository<WarehouseItemReadModel> itemReadRepository)
        {
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _itemReadRepository = itemReadRepository ?? throw new ArgumentNullException(nameof(itemReadRepository));
        }

        public IEnumerable<WarehouseReadModel> GetAll(Expression<Func<WarehouseReadModel, bool>> predicate)
        {
            return _readRepository.GetAll(predicate);
        }

        public IEnumerable<WarehouseItemReadModel> GetAllItems(Expression<Func<WarehouseItemReadModel, bool>> predicate)
        {
            return _itemReadRepository.GetAll(predicate);
        }

        public async Task<WarehouseReadModel> GetByIdAsync(Guid id)
        {
            return await _readRepository.GetByIdAsync(id);
        }

        public async Task<WarehouseAggregate> GetByIdESAsync(Guid id)
        {
            var warehouseAggregate = await _eventStoreRepository.GetByIdAsync(id);

            return warehouseAggregate;
        }
    }
}
