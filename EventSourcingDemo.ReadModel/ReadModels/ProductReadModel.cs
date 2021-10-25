using System;
using System.Collections.Generic;

namespace EventSourcingDemo.ReadModel.ReadModels
{
    public class ProductReadModel : IReadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<WarehouseItemReadModel> WarehouseItems { get; set; }
    }
}
