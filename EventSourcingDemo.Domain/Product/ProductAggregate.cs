using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Product.Events;
using System;

namespace EventSourcingDemo.Domain.Product
{
    public class ProductAggregate : AggregateBase
    {
        private string Name { get; set; }
        private string Description { get; set; }
        private decimal Price { get; set; }
        private DateTime LastModified { get; set; }

        private ProductAggregate() { }

        public ProductAggregate(string name, string description, decimal price)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException(nameof(description));

            AggregateId = Guid.NewGuid();

            var @event = new ProductCreatedEvent
            {
                Name = name,
                Description = description,
                Price = price
            };

            RaiseEvent(@event);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var @event = new ProductNameChangedEvent
            {
                NewName = name,
                OldName = Name,
                LastModifiedAt = DateTime.Now
            };

            RaiseEvent(@event);
        }

        public void DeleteProduct(string reason)
        {
            if (string.IsNullOrEmpty(reason)) throw new ArgumentNullException(nameof(reason));

            var @event = new ProductDeletedEvent
            {
                DeletedAt = DateTime.Now,
                Reason = reason
            };

            RaiseEvent(@event);
        }

        #region Apply events

        public void Apply(ProductCreatedEvent @event)
        {
            AggregateId = @event.AggregateId;
            Name = @event.Name;
            Description = @event.Description;
            Price = @event.Price;
        }

        public void Apply(ProductNameChangedEvent @event)
        {
            Name = @event.NewName;
            LastModified = @event.LastModifiedAt;
        }

        public void Apply(ProductDeletedEvent @event)
        {
            IsDeleted = true;
            LastModified = @event.DeletedAt;
        }

        #endregion
    }
}
