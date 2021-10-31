using EventSourcingDemo.ReadModel.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Product
{
    public interface IProductReader
    {
        Task<ProductReadModel> GetByIdAsync(Guid id);

        IEnumerable<ProductReadModel> GetAll(Expression<Func<ProductReadModel, bool>> predicate);
    }
}
