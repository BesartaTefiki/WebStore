using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        // Allow anonymous: shop can be browsed without login
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        // All roles can manage products: admin, advanced, simple
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        // PUT: api/products/5
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            await _productService.UpdateAsync(product);
            return NoContent();
        }

        // DELETE: api/products/5
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }


        // PUT: api/products/5/discount (admin)
        [Authorize(Roles = "admin")]
        [HttpPut("{id}/discount")]
        public async Task<IActionResult> SetDiscount(int id, [FromBody] DiscountRequestDto dto)
        {
            await _productService.ApplyDiscountAsync(id, dto.DiscountPercent);
            return NoContent();
        }

      
        // GET: api/products/search?genderId=1&categoryId=2&brandId=3&priceMin=10&priceMax=100&sizeId=1&colorId=2&inStock=true
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
            [FromQuery] int? categoryId,
            [FromQuery] int? genderId,
            [FromQuery] int? brandId,
            [FromQuery] decimal? priceMin,
            [FromQuery] decimal? priceMax,
            [FromQuery] int? sizeId,
            [FromQuery] int? colorId,
            [FromQuery] bool? inStock
        )
        {
            var products = await _productService.SearchAsync(
                categoryId,
                genderId,
                brandId,
                priceMin,
                priceMax,
                sizeId,
                colorId,
                inStock
            );

            return Ok(products);
        }

        // GET: api/products/5/quantity
        [AllowAnonymous]
        [HttpGet("{id}/quantity")]
        public async Task<IActionResult> GetProductQuantity(int id)
        {
            ProductQuantityDto? dto = await _productService.GetQuantityAsync(id);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }
    }
}
