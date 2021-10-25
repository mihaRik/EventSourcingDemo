using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.ReadModel.Repository
{
    public interface IWarehouseItemRepository : IReadRepository<WarehouseItemReadModel>,
                                                  IWriteRepository<WarehouseItemReadModel>
    {
        Task<WarehouseItemReadModel> GetSingleAsync(Expression<Func<WarehouseItemReadModel, bool>> predicate);
    }
}
