using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Product.Events
{
    public class ProductDeletedEvent : EventBase
    {
        public DateTime DeletedAt { get; set; }
        public string Reason { get; set; }
    }
}
