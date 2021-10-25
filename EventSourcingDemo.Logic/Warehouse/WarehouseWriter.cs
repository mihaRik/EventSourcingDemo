using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Models;
using EventSourcingDemo.Domain.PubSub;
using EventSourcingDemo.Domain.Warehouse;
using EventSourcingDemo.Domain.Warehouse.Events;
using EventSourcingDemo.EventStore.Repository;
using EventSourcingDemo.ReadModel;
using EventSourcingDemo.ReadModel.ReadModels;
using EventSourcingDemo.ReadModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Warehouse
{
    public class WarehouseWriter : IWarehouseWriter
    {
        private readonly IEventStoreRepository<WarehouseAggregate> _eventStoreRepositroy;
        private readonly IReadRepository<ProductReadModel> _productReadRepository;
        private readonly IEventSubscriber _eventSubscriber;
        private readonly IEnumerable<IEventHandler<WarehouseCreatedEvent>> _warehouseCreatedEventHandlers;
        private readonly IEnumerable<IEventHandler<WarehouseNameChangedEvent>> _warehouseNameChangedEventHandlers;
        private readonly IEnumerable<IEventHandler<ItemAddedToWarehouseEvent>> _itemAddedToWarehouseEventHandlers;
        private readonly IEnumerable<IEventHandler<ExistingItemAddedToWarehouseEvent>> _existingItemAddedToWarehouseEventHandlers;
        private readonly IEnumerable<IEventHandler<WithdrawnEvent>> _withdrawnEventHandlers;
        private readonly IEnumerable<IEventHandler<WarehouseDeletedEvent>> _warehouseDeletedEventHandlers;
        private readonly IEnumerable<IEventHandler<WarehouseItemDeletedEvent>> _warehouseItemDeletedEventHandlers;

        public WarehouseWriter(IEventStoreRepository<WarehouseAggregate> eventStoreRepositroy,
                               IEventSubscriber eventSubscriber,
                               IReadRepository<ProductReadModel> productReadRepository,
                               IEnumerable<IEventHandler<WarehouseCreatedEvent>> warehouseCreatedEventHandlers,
                               IEnumerable<IEventHandler<WarehouseNameChangedEvent>> warehouseNameChangedEventHandlers,
                               IEnumerable<IEventHandler<ItemAddedToWarehouseEvent>> itemAddedToWarehouseEventHandlers,
                               IEnumerable<IEventHandler<ExistingItemAddedToWarehouseEvent>> existingItemAddedToWarehouseEventHandlers,
                               IEnumerable<IEventHandler<WithdrawnEvent>> withdrawnEventHandlers,
                               IEnumerable<IEventHandler<WarehouseDeletedEvent>> warehouseDeletedEventHandlers,
                               IEnumerable<IEventHandler<WarehouseItemDeletedEvent>> warehouseItemDeletedEventHandlers)
        {
            _eventStoreRepositroy = eventStoreRepositroy ?? throw new ArgumentNullException(nameof(eventStoreRepositroy));
            _eventSubscriber = eventSubscriber ?? throw new ArgumentNullException(nameof(eventSubscriber));
            _productReadRepository = productReadRepository ?? throw new ArgumentNullException(nameof(productReadRepository));
            _warehouseCreatedEventHandlers = warehouseCreatedEventHandlers;
            _warehouseNameChangedEventHandlers = warehouseNameChangedEventHandlers;
            _itemAddedToWarehouseEventHandlers = itemAddedToWarehouseEventHandlers;
            _existingItemAddedToWarehouseEventHandlers = existingItemAddedToWarehouseEventHandlers;
            _withdrawnEventHandlers = withdrawnEventHandlers;
            _warehouseDeletedEventHandlers = warehouseDeletedEventHandlers;
            _warehouseItemDeletedEventHandlers = warehouseItemDeletedEventHandlers;
        }

        public async Task AddItemAsync(Guid warehouseId, Guid productId, int quantity)
        {
            var warehouseAggregate = await _eventStoreRepositroy.GetByIdAsync(warehouseId);

            var product = await _productReadRepository.GetByIdAsync(productId);

            var warehouseItem = new WarehouseItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Quantity = quantity,
                Price = product.Price
            };

            warehouseAggregate.AddItem(warehouseItem);

            _eventSubscriber.Subscribe<ItemAddedToWarehouseEvent>(async e =>
                                            await HandleEvents(_itemAddedToWarehouseEventHandlers, e));

            _eventSubscriber.Subscribe<ExistingItemAddedToWarehouseEvent>(async e =>
                                            await HandleEvents(_existingItemAddedToWarehouseEventHandlers, e));

            await _eventStoreRepositroy.SaveAsync(warehouseAggregate);
        }

        public async Task ChangeWarehouseNameAsync(Guid warehouseId, string name)
        {
            var warehouseAggregate = await _eventStoreRepositroy.GetByIdAsync(warehouseId);

            warehouseAggregate.ChangeName(name);

            _eventSubscriber.Subscribe<WarehouseNameChangedEvent>(async e => 
                                                                  await HandleEvents(_warehouseNameChangedEventHandlers, e));

            await _eventStoreRepositroy.SaveAsync(warehouseAggregate);
        }

        public async Task<Guid> CreateAsync(string name)
        {
            var newWarehouse = new WarehouseAggregate(name);

            _eventSubscriber.Subscribe<WarehouseCreatedEvent>(async e => await HandleEvents(_warehouseCreatedEventHandlers, e));

            await _eventStoreRepositroy.SaveAsync(newWarehouse);

            return newWarehouse.AggregateId;
        }

        public async Task WithdrawItemAsync(Guid warehouseId, Guid itemId, int quantity, string reason)
        {
            var warehouseAggregate = await _eventStoreRepositroy.GetByIdAsync(warehouseId);

            warehouseAggregate.WithdrawItem(itemId, quantity, reason);

            _eventSubscriber.Subscribe<WithdrawnEvent>(async e => await HandleEvents(_withdrawnEventHandlers, e));

            await _eventStoreRepositroy.SaveAsync(warehouseAggregate);
        }
 
        public async Task DeleteWarehouseAsync(Guid warehouseId, string reason)
        {
            var warehouseAggregate = await _eventStoreRepositroy.GetByIdAsync(warehouseId);

            warehouseAggregate.DeleteWarehouse(reason);

            _eventSubscriber.Subscribe<WarehouseDeletedEvent>(async e =>
                                                              await HandleEvents(_warehouseDeletedEventHandlers, e));

            await _eventStoreRepositroy.SaveAsync(warehouseAggregate);
        }

        public async Task DeleteWarehouseItemAsync(Guid warehouseId, Guid itemId, string reason)
        {
            var warehouseAggregate = await _eventStoreRepositroy.GetByIdAsync(warehouseId);

            warehouseAggregate.DeleteWarehouseItem(itemId, reason);

            _eventSubscriber.Subscribe<WarehouseItemDeletedEvent>(async e =>
                                                    await HandleEvents(_warehouseItemDeletedEventHandlers, e));

            await _eventStoreRepositroy.SaveAsync(warehouseAggregate);
        }
        private static async Task HandleEvents<T>(IEnumerable<IEventHandler<T>> handlers, T @event) where T : IEvent
        {
            foreach (var handler in handlers ?? Enumerable.Empty<IEventHandler<T>>())
            {
                await handler.HandleAsync(@event);
            }
        }
    }
}
