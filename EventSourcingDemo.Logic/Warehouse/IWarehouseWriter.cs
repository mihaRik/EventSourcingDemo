using System;
using System.Threading.Tasks;

namespace EventSourcingDemo.Logic.Warehouse
{
    public interface IWarehouseWriter
    {
        Task<Guid> CreateAsync(string name);
        Task AddItemAsync(Guid warehouseId, Guid productId, int quantity);
        Task WithdrawItemAsync(Guid warehouseId, Guid itemId, int quantity, string reason);
        Task ChangeWarehouseNameAsync(Guid warehouseId, string name);
        Task DeleteWarehouseAsync(Guid warehouseId, string reason);
        Task DeleteWarehouseItemAsync(Guid warehouseId, Guid itemId, string reason);
    }
}
