using EventSourcingDemo.ReadModel.ReadModels;
using EventSourcingDemo.ReadModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Product
{
    public class ProductReader : IProductReader
    {
        private readonly IReadRepository<ProductReadModel> _readRepository;

        public ProductReader(IReadRepository<ProductReadModel> readRepository)
        {
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        }

        public IEnumerable<ProductReadModel> GetAll(Expression<Func<ProductReadModel, bool>> predicate)
        {
            return _readRepository.GetAll(predicate);
        }

        public async Task<ProductReadModel> GetByIdAsync(Guid id)
        {
            return await _readRepository.GetByIdAsync(id);
        }
    }
}
