using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse
{
    public class WarehouseNameChangedEvent : EventBase
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
