using EventSourcingDemo.Logic.Product;
using EventSourcingDemo.ReadModel.ReadModels;
using EventSourcingDemo.ReadModel.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingDemo.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductReader _productReader;
        private readonly IProductWriter _productWriter;

        public ProductController(IProductReader productReader,
                                 IProductWriter productWriter)
        {
            _productReader = productReader ?? throw new ArgumentNullException(nameof(productReader));
            _productWriter = productWriter ?? throw new ArgumentNullException(nameof(productWriter));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _productReader.GetAll(_ => true);

            return Ok(items.Select(_ => new
            {
                _.Id,
                _.Description,
                _.Name,
                _.Price
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _productReader.GetByIdAsync(id);

            return Ok(new
            {
                item.Id,
                item.Description,
                item.Name,
                item.Price
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description, decimal price)
        {
            var productId = await _productWriter.CreateProductAsync(name, description, price);

            return Ok(productId);
        }

        [HttpPut("{id}/name")]
        public async Task<IActionResult> ChangeProductName(Guid id, string newName)
        {
            await _productWriter.ChangeProductNameAsync(id, newName);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id, string reason)
        {
            await _productWriter.DeleteProductAsync(id, reason);

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
