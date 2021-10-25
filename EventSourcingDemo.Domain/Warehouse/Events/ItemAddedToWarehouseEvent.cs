using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse
{
    public class ItemAddedToWarehouseEvent : EventBase
    {
        public Guid ItemId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
