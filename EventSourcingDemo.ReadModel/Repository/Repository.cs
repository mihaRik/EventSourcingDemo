using EventSourcingDemo.ReadModel.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.ReadModel.Repository
{
    public class Repository<T> : IReadRepository<T>, IWriteRepository<T> where T : class, IReadModel
    {
        protected readonly EventSourcingReadModelDbContext _db;

        public Repository(EventSourcingReadModelDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task DeleteAsync(T entity)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entites)
        {
            _db.Set<T>().RemoveRange(entites);
            await _db.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _db.Set<T>().Where(predicate);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task InsertAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entites)
        {
            _db.Set<T>().UpdateRange(entites);
            await _db.SaveChangesAsync();
        }
    }
}
