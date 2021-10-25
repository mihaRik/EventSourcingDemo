using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.ReadModel.Repository
{
    public interface IReadRepository<T> where T : IReadModel
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(Guid id);
    }
}
