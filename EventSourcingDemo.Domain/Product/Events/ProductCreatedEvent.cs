using EventSourcingDemo.Domain.Base;

namespace EventSourcingDemo.Domain.Product.Events
{
    public class ProductCreatedEvent : EventBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
