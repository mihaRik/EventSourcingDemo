using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EventSourcingDemo.Logic.Warehouse;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace EventSourcingDemo.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseWriter _warehouseWriter;
        private readonly IWarehouseReader _warehouseReader;

        public WarehouseController(IWarehouseWriter warehouseWriter,
                                   IWarehouseReader warehouseReader)
        {
            _warehouseWriter = warehouseWriter ?? throw new ArgumentNullException(nameof(warehouseWriter));
            _warehouseReader = warehouseReader ?? throw new ArgumentNullException(nameof(warehouseReader));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var warehouses = _warehouseReader.GetAll(_ => true);

            return Ok(warehouses.Select(_ => new
            {
                _.Id,
                _.Name,
                _.ItemsQuantity
            }));
        }

        [HttpGet("materialized-view/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var warehouse = await _warehouseReader.GetByIdAsync(id);

            return Ok(new
            {
                warehouse.Id,
                warehouse.Name,
                warehouse.ItemsQuantity
            });
        }

        [HttpGet("event-sourcing/{id}")]
        public async Task<IActionResult> GetByIdES(Guid id)
        {
            var warehouse = await _warehouseReader.GetByIdESAsync(id);

            return Ok(warehouse);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var id = await _warehouseWriter.CreateAsync(name);

            return Ok(id);
        }

        [HttpPut("{warehouseId}")]
        public async Task<IActionResult> ChangeName(Guid warehouseId, string name)
        {
            await _warehouseWriter.ChangeWarehouseNameAsync(warehouseId, name);

            return Ok();
        }

        [HttpDelete("{warehouseId}")]
        public async Task<IActionResult> DeleteById(Guid warehouseId, string reason)
        {
            await _warehouseWriter.DeleteWarehouseAsync(warehouseId, reason);

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
