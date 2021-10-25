using EventSourcingDemo.ReadModel.ReadModels;
using System;

namespace EventSourcingDemo.ReadModel
{
    public class WarehouseItemReadModel : IReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual WarehouseReadModel Warehouse { get; set; }
        public virtual ProductReadModel Product { get; set; }

        public Guid WarehouseId { get; set; }
        public Guid ProductId { get; set; }
    }
}
