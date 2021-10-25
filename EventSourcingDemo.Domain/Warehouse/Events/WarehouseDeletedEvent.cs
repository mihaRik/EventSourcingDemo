
using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse.Events
{
    public class WarehouseDeletedEvent : EventBase
    {
        public DateTime DeletedAt { get; set; }
        public string Reason { get; set; }
    }
}
