using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse.Events
{
    public class ExistingItemAddedToWarehouseEvent : EventBase
    {
        public Guid ItemId { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityAdded { get; set; }
    }
}
