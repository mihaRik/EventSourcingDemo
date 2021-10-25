using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcingDemo.ReadModel.Repository
{
    public interface IWriteRepository<T> where T : IReadModel
    {
        Task InsertAsync(T entity);

        Task UpdateAsync(T entity);

        Task UpdateRangeAsync(IEnumerable<T> entites);

        Task DeleteAsync(T entity);

        Task DeleteRangeAsync(IEnumerable<T> entites);
    }
}
