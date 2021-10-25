using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Product.Events;
using EventSourcingDemo.Domain.Warehouse;
using EventSourcingDemo.Domain.Warehouse.Events;
using EventSourcingDemo.ReadModel;
using EventSourcingDemo.ReadModel.ReadModels;
using EventSourcingDemo.ReadModel.Repository;
using System;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Warehouse
{
    public class WarehouseUpdater : IEventHandler<WarehouseCreatedEvent>,
                                    IEventHandler<WarehouseNameChangedEvent>,
                                    IEventHandler<ItemAddedToWarehouseEvent>,
                                    IEventHandler<ExistingItemAddedToWarehouseEvent>,
                                    IEventHandler<WithdrawnEvent>,
                                    IEventHandler<ProductNameChangedEvent>,
                                    IEventHandler<WarehouseDeletedEvent>,
                                    IEventHandler<WarehouseItemDeletedEvent>
    {
        private readonly IWriteRepository<WarehouseReadModel> _writeRepository;
        private readonly IReadRepository<WarehouseReadModel> _readRepository;
        private readonly IWarehouseItemRepository _warehouseItemRepository;

        public WarehouseUpdater(IWriteRepository<WarehouseReadModel> writeRepository,
                                IReadRepository<WarehouseReadModel> readRepository,
                                IWarehouseItemRepository warehouseItemRepository)
        {
            _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            _warehouseItemRepository = warehouseItemRepository ?? throw new ArgumentNullException(nameof(warehouseItemRepository));
        }

        public async Task HandleAsync(WarehouseCreatedEvent @event)
        {
            var newWarehouse = new WarehouseReadModel
            {
                Id = @event.AggregateId,
                Name = @event.Name,
                ItemsQuantity = 0
            };

            await _writeRepository.InsertAsync(newWarehouse);
        }

        public async Task HandleAsync(WarehouseNameChangedEvent @event)
        {
            var warehouse = await _readRepository.GetByIdAsync(@event.AggregateId);

            warehouse.Name = @event.NewName;

            await _writeRepository.UpdateAsync(warehouse);
        }

        public async Task HandleAsync(ItemAddedToWarehouseEvent @event)
        {
            var warehouse = await _readRepository.GetByIdAsync(@event.AggregateId);

            warehouse.ItemsQuantity += @event.Quantity;

            var warehouseItem = new WarehouseItemReadModel
            {
                Id = @event.ItemId,
                Name = @event.Name,
                Price = @event.Price,
                Quantity = @event.Quantity,
                WarehouseId = @event.AggregateId,
                ProductId = @event.ProductId
            };

            await _writeRepository.UpdateAsync(warehouse);
            await _warehouseItemRepository.InsertAsync(warehouseItem);
        }

        public async Task HandleAsync(ExistingItemAddedToWarehouseEvent @event)
        {
            var warehouse = await _readRepository.GetByIdAsync(@event.AggregateId);
            var warehouseItem = await _warehouseItemRepository.GetSingleAsync(_ => _.ProductId == @event.ProductId &&
                                                                                   _.WarehouseId == @event.AggregateId);

            warehouse.ItemsQuantity += @event.QuantityAdded;
            warehouseItem.Quantity += @event.QuantityAdded;

            await _writeRepository.UpdateAsync(warehouse);
            await _warehouseItemRepository.UpdateAsync(warehouseItem);
        }

        public async Task HandleAsync(WithdrawnEvent @event)
        {
            var warehouse = await _readRepository.GetByIdAsync(@event.AggregateId);
            var warehouseItem = await _warehouseItemRepository.GetByIdAsync(@event.ItemId);

            warehouse.ItemsQuantity -= @event.Quantity;
            warehouseItem.Quantity -= @event.Quantity;

            if (warehouseItem.Quantity == 0)
            {
                await _warehouseItemRepository.DeleteAsync(warehouseItem);
            }
            else
            {
                await _warehouseItemRepository.UpdateAsync(warehouseItem);
            }

            await _writeRepository.UpdateAsync(warehouse);
        }

        public async Task HandleAsync(ProductNameChangedEvent @event)
        {
            var warehouseItems = _warehouseItemRepository.GetAll(_ => _.ProductId == @event.AggregateId);

            foreach (var item in warehouseItems)
            {
                item.Name = @event.NewName;
            }

            await _warehouseItemRepository.UpdateRangeAsync(warehouseItems);
        }

        public async Task HandleAsync(WarehouseDeletedEvent @event)
        {
            var warehouse = await _readRepository.GetByIdAsync(@event.AggregateId);

            await _writeRepository.DeleteAsync(warehouse);
        }

        public async Task HandleAsync(WarehouseItemDeletedEvent @event)
        {
            var warehouseItem = await _warehouseItemRepository.GetByIdAsync(@event.ItemId);
            var warehouse = await _readRepository.GetByIdAsync(@event.AggregateId);

            warehouse.ItemsQuantity -= warehouseItem.Quantity;

            await _writeRepository.UpdateAsync(warehouse);
            await _warehouseItemRepository.DeleteAsync(warehouseItem);
        }
    }
}
