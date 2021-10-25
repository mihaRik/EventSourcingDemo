using System;
using System.Collections.Generic;

namespace EventSourcingDemo.ReadModel
{
    public class WarehouseReadModel : IReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ItemsQuantity { get; set; }

        public virtual ICollection<WarehouseItemReadModel> Items { get; set; }
    }
}
