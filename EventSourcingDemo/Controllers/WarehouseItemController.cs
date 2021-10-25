using EventSourcingDemo.Logic.Warehouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingDemo.Controllers
{
    [ApiController]
    [Route("api/warehouse/{warehouseId}/items")]
    public class WarehouseItemController : ControllerBase
    {
        private readonly IWarehouseWriter _warehouseWriter;
        private readonly IWarehouseReader _warehouseReader;

        public WarehouseItemController(IWarehouseWriter warehouseWriter,
                                       IWarehouseReader warehouseReader)
        {
            _warehouseWriter = warehouseWriter ?? throw new ArgumentNullException(nameof(warehouseWriter));
            _warehouseReader = warehouseReader ?? throw new ArgumentNullException(nameof(warehouseReader));
        }

        [HttpGet]
        public IActionResult GetAll(Guid warehouseId)
        {
            var items = _warehouseReader.GetAllItems(_ => _.WarehouseId == warehouseId);

            return Ok(items.Select(_ => new
            {
                _.Id,
                _.Name,
                _.Price,
                _.Quantity,
                _.ProductId,
                _.WarehouseId
            }));
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToWarehouse(Guid warehouseId, Guid productId, int quantity)
        {
            await _warehouseWriter.AddItemAsync(warehouseId, productId, quantity);

            return Ok();
        }

        [HttpPost("{itemId}/withdraw")]
        public async Task<IActionResult> WithdrawItemFromWarehouse(Guid warehouseId, Guid itemId, int quantity, string reason)
        {
            await _warehouseWriter.WithdrawItemAsync(warehouseId, itemId, quantity, reason);

            return Ok();
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteItemFromWarehouse(Guid warehouseId, Guid itemId, string reason)
        {
            await _warehouseWriter.DeleteWarehouseItemAsync(warehouseId, itemId, reason);

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
