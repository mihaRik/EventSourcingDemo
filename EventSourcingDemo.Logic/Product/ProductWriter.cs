using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Product;
using EventSourcingDemo.Domain.Product.Events;
using EventSourcingDemo.Domain.PubSub;
using EventSourcingDemo.EventStore.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Product
{
    public class ProductWriter : IProductWriter
    {
        private readonly IEventSubscriber _eventSubscriber;
        private readonly IEventStoreRepository<ProductAggregate> _eventStoreRepository;
        private readonly IEnumerable<IEventHandler<ProductCreatedEvent>> _productCreatedEventHandlers;
        private readonly IEnumerable<IEventHandler<ProductNameChangedEvent>> _productNameChangedEventHandlers;
        private readonly IEnumerable<IEventHandler<ProductDeletedEvent>> _productDeletedEventHandlers;

        public ProductWriter(IEventSubscriber eventSubscriber,
                             IEventStoreRepository<ProductAggregate> eventStoreRepository,
                             IEnumerable<IEventHandler<ProductCreatedEvent>> productCreatedEventHandlers,
                             IEnumerable<IEventHandler<ProductNameChangedEvent>> productNameChangedEventHandlers,
                             IEnumerable<IEventHandler<ProductDeletedEvent>> productDeletedEventHandlers)
        {
            _eventSubscriber = eventSubscriber ?? throw new ArgumentNullException(nameof(eventSubscriber));
            _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _productCreatedEventHandlers = productCreatedEventHandlers;
            _productNameChangedEventHandlers = productNameChangedEventHandlers;
            _productDeletedEventHandlers = productDeletedEventHandlers;
        }

        public async Task ChangeProductNameAsync(Guid productId, string newName)
        {
            var productAggregate = await _eventStoreRepository.GetByIdAsync(productId);

            productAggregate.ChangeName(newName);

            _eventSubscriber.Subscribe<ProductNameChangedEvent>(async e => await HandleEvents(_productNameChangedEventHandlers, e));

            await _eventStoreRepository.SaveAsync(productAggregate);
        }

        public async Task<Guid> CreateProductAsync(string name, string description, decimal price)
        {
            var productAggregate = new ProductAggregate(name, description, price);

            _eventSubscriber.Subscribe<ProductCreatedEvent>(async e => await HandleEvents(_productCreatedEventHandlers, e));

            await _eventStoreRepository.SaveAsync(productAggregate);

            return productAggregate.AggregateId;
        }

        public async Task DeleteProductAsync(Guid id, string reason)
        {
            var productAggregate = await _eventStoreRepository.GetByIdAsync(id);

            productAggregate.DeleteProduct(reason);

            _eventSubscriber.Subscribe<ProductDeletedEvent>(async e => 
                                                            await HandleEvents(_productDeletedEventHandlers, e));

            await _eventStoreRepository.SaveAsync(productAggregate);
        }

        private async Task HandleEvents<T>(IEnumerable<IEventHandler<T>> handlers, T @event) where T : IEvent
        {
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(@event);
            }
        }
    }
}
