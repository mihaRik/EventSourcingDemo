using EventSourcingDemo.ReadModel.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.ReadModel.Repository
{
    public class WarehouseItemRepository : Repository<WarehouseItemReadModel>, IWarehouseItemRepository
    {
        public WarehouseItemRepository(EventSourcingReadModelDbContext db) : base(db)
        { }

        public async Task<WarehouseItemReadModel> GetSingleAsync(Expression<Func<WarehouseItemReadModel, bool>> predicate)
        {
            return await _db.WarehouseItems.SingleOrDefaultAsync(predicate);
        }
    }
}
