using EventSourcingDemo.Domain.Base;
using System;

namespace EventSourcingDemo.Domain.Product.Events
{
    public class ProductNameChangedEvent : EventBase
    {
        public string NewName { get; set; }
        public string OldName { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
