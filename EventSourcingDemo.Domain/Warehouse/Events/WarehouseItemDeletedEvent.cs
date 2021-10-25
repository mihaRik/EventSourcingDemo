using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse.Events
{
    public class WarehouseItemDeletedEvent : EventBase
    {
        public Guid ItemId { get; set; }
        public DateTime DeletedAt { get; set; }
        public string Reason { get; set; }
    }
}
