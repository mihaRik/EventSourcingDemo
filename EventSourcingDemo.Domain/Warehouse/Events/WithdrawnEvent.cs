using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Warehouse.Events
{
    public class WithdrawnEvent : EventBase
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }
}
