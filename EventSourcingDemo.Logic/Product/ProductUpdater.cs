using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Product.Events;
using EventSourcingDemo.ReadModel;
using EventSourcingDemo.ReadModel.ReadModels;
using EventSourcingDemo.ReadModel.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Product
{
    public class ProductUpdater : IEventHandler<ProductCreatedEvent>,
                                  IEventHandler<ProductNameChangedEvent>,
                                  IEventHandler<ProductDeletedEvent>
    {
        private readonly IWriteRepository<ProductReadModel> _writeRepository;
        private readonly IReadRepository<ProductReadModel> _readRepository;
        private readonly IWriteRepository<WarehouseReadModel> _warehouseReadRepository;
        private readonly IWriteRepository<WarehouseItemReadModel> _warehouseItemReadRepository;

        public ProductUpdater(IWriteRepository<ProductReadModel> writeRepository,
                              IReadRepository<ProductReadModel> readRepository,
                              IWriteRepository<WarehouseReadModel> warehouseReadRepository,
                              IWriteRepository<WarehouseItemReadModel> warehouseItemReadRepository)
        {
            _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            _warehouseReadRepository = warehouseReadRepository ?? throw new ArgumentNullException(nameof(warehouseReadRepository));
            _warehouseItemReadRepository = warehouseItemReadRepository ?? throw new ArgumentNullException(nameof(warehouseItemReadRepository));
        }

        public async Task HandleAsync(ProductCreatedEvent @event)
        {
            var product = new ProductReadModel
            {
                Id = @event.AggregateId,
                Name = @event.Name,
                Description = @event.Description,
                Price = @event.Price
            };

            await _writeRepository.InsertAsync(product);
        }

        public async Task HandleAsync(ProductNameChangedEvent @event)
        {
            var product = await _readRepository.GetByIdAsync(@event.AggregateId);

            product.Name = @event.NewName;

            await _writeRepository.UpdateAsync(product);
        }

        public async Task HandleAsync(ProductDeletedEvent @event)
        {
            var product = await _readRepository.GetByIdAsync(@event.AggregateId);

            foreach (var warehouseItem in product.WarehouseItems)
            {
                warehouseItem.Warehouse.ItemsQuantity -= warehouseItem.Quantity;
            }

            await _warehouseReadRepository.UpdateRangeAsync(product.WarehouseItems.Select(_ => _.Warehouse));

            await _warehouseItemReadRepository.DeleteRangeAsync(product.WarehouseItems);

            await _writeRepository.DeleteAsync(product);
        }
    }
}
