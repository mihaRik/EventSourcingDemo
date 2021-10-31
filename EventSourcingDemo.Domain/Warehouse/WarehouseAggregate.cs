using EventSourcingDemo.Domain.Base;
using EventSourcingDemo.Domain.Models;
using EventSourcingDemo.Domain.Warehouse.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcingDemo.Domain.Warehouse
{
    public class WarehouseAggregate : AggregateBase
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public ICollection<WarehouseItem> Items { get; set; } = new List<WarehouseItem>();

        private WarehouseAggregate() { }

        public WarehouseAggregate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            AggregateId = Guid.NewGuid();

            var @event = new WarehouseCreatedEvent
            {
                Name = name,
                CreatedAt = DateTime.Now
            };

            RaiseEvent(@event);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var @event = new WarehouseNameChangedEvent
            {
                NewName = name,
                ModifiedAt = DateTime.Now,
                OldName = this.Name
            };

            RaiseEvent(@event);
        }

        public void AddItem(WarehouseItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("The invalid quantity of item");
            }

            IEvent @event;

            if (Items.Any(_ => _.ProductId == item.ProductId))
            {
                @event = new ExistingItemAddedToWarehouseEvent
                {
                    ItemId = Items.SingleOrDefault(_ => _.ProductId == item.ProductId).Id,
                    ProductId = item.ProductId,
                    QuantityAdded = item.Quantity
                };
            }
            else
            {
                @event = new ItemAddedToWarehouseEvent
                {
                    ItemId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
            }


            RaiseEvent(@event);
        }

        public void WithdrawItem(Guid itemId, int quantity, string reason)
        {
            if (itemId == Guid.Empty) throw new ArgumentNullException(nameof(itemId));
            if (quantity <= 0) throw new InvalidOperationException("Withdraw quantity should be more than 0");

            var item = Items.SingleOrDefault(_ => _.Id == itemId);

            if (item == null) throw new KeyNotFoundException(nameof(item));

            if (item.Quantity - quantity < 0) throw new InvalidOperationException("You cannot withdraw more than you have in warehouse");

            var @event = new WithdrawnEvent
            {
                ItemId = itemId,
                Quantity = quantity,
                Reason = reason
            };

            RaiseEvent(@event);
        }

        public void DeleteWarehouse(string reason)
        {
            if (string.IsNullOrEmpty(reason)) throw new ArgumentNullException(nameof(reason));

            var @event = new WarehouseDeletedEvent
            {
                DeletedAt = DateTime.Now,
                Reason = reason
            };

            RaiseEvent(@event);
        }

        public void DeleteWarehouseItem(Guid itemId, string reason)
        {
            if (itemId == Guid.Empty) throw new ArgumentNullException(nameof(itemId));
            if (string.IsNullOrEmpty(reason)) throw new ArgumentNullException(nameof(reason));

            var item = Items.SingleOrDefault(_ => _.Id == itemId);

            if (item == null) throw new KeyNotFoundException(nameof(item));

            var @event = new WarehouseItemDeletedEvent
            {
                ItemId = item.Id,
                DeletedAt = DateTime.Now,
                Reason = reason
            };

            RaiseEvent(@event);
        }

        #region Apply events

        public void Apply(WarehouseCreatedEvent @event)
        {
            AggregateId = @event.AggregateId;
            Name = @event.Name;
            CreatedAt = @event.CreatedAt;
            LastModifiedAt = DateTime.Now;
        }

        public void Apply(WarehouseNameChangedEvent @event)
        {
            Name = @event.NewName;
            LastModifiedAt = @event.ModifiedAt;
        }

        public void Apply(ItemAddedToWarehouseEvent @event)
        {
            var item = new WarehouseItem
            {
                Id = @event.ItemId,
                Name = @event.Name,
                Quantity = @event.Quantity,
                Price = @event.Price,
                ProductId = @event.ProductId
            };

            Items.Add(item);
        }

        public void Apply(ExistingItemAddedToWarehouseEvent @event)
        {
            var item = Items.SingleOrDefault(_ => _.Id == @event.ItemId);

            if (item == null) throw new KeyNotFoundException(nameof(item));

            item.Quantity += @event.QuantityAdded;
        }

        public void Apply(WithdrawnEvent @event)
        {
            var item = Items.SingleOrDefault(_ => _.Id == @event.ItemId);

            if (item == null) throw new KeyNotFoundException(nameof(item));

            item.Quantity -= @event.Quantity;

            if (item.Quantity == 0)
            {
                Items.Remove(item);
            }
        }

        public void Apply(WarehouseDeletedEvent @event)
        {
            IsDeleted = true;
            LastModifiedAt = @event.DeletedAt;
        }

        public void Apply(WarehouseItemDeletedEvent @event)
        {
            var item = Items.SingleOrDefault(_ => _.Id ==  @event.ItemId);

            if (item == null) throw new KeyNotFoundException(nameof(item));

            Items.Remove(item);
            LastModifiedAt = @event.DeletedAt;
        }

        #endregion
    }
}
