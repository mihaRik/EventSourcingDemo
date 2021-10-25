using EventSourcingDemo.Domain.Warehouse;
using EventSourcingDemo.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Warehouse
{
    public interface IWarehouseReader
    {
        Task<WarehouseReadModel> GetByIdAsync(Guid id);
        Task<WarehouseAggregate> GetByIdESAsync(Guid id);

        IEnumerable<WarehouseReadModel> GetAll(Expression<Func<WarehouseReadModel, bool>> predicate);

        IEnumerable<WarehouseItemReadModel> GetAllItems(Expression<Func<WarehouseItemReadModel, bool>> predicate);
    }
}
