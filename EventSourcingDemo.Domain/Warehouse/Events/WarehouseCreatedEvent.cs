using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse
{
    public class WarehouseCreatedEvent : EventBase
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
