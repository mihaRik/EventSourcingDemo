using System;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Product
{
    public interface IProductWriter
    {
        Task<Guid> CreateProductAsync(string name, string description, decimal price);
        Task ChangeProductNameAsync(Guid productId, string newName);
        Task DeleteProductAsync(Guid id, string reason);
    }
}
