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

        /// <summary>
        /// Returns all products.
        /// </summary>
        /// <remarks>
        /// GET /api/products  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Returns a product by id.
        /// </summary>
        /// <remarks>
        /// GET /api/products/{id}  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <remarks>
        /// POST /api/products  
        /// Roles allowed: admin, advanced, simple.  
        /// Clients cannot modify products.
        /// </remarks>
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var clientClaim = User.FindFirst("client_id")?.Value;
            if (!string.IsNullOrEmpty(clientClaim))
                return Forbid("Clients cannot manage products.");

            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <remarks>
        /// PUT /api/products/{id}  
        /// Roles allowed: admin, advanced, simple.
        /// </remarks>
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            await _productService.UpdateAsync(product);
            return NoContent();
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <remarks>
        /// DELETE /api/products/{id}  
        /// Roles allowed: admin, advanced, simple.
        /// </remarks>
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Applies discount to a product.
        /// </summary>
        /// <remarks>
        /// PUT /api/products/{id}/discount  
        /// Only admin can apply discounts.
        /// </remarks>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}/discount")]
        public async Task<IActionResult> SetDiscount(int id, [FromBody] DiscountRequestDto dto)
        {
            await _productService.ApplyDiscountAsync(id, dto.DiscountPercent);
            return NoContent();
        }

        /// <summary>
        /// Searches products using optional filters.
        /// </summary>
        /// <remarks>
        /// GET /api/products/search  
        /// Public endpoint.
        /// </remarks>
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
            [FromQuery] bool? inStock)
        {
            var products = await _productService.SearchAsync(
                categoryId, genderId, brandId,
                priceMin, priceMax, sizeId, colorId, inStock);

            return Ok(products);
        }

        /// <summary>
        /// Returns stock quantity for a product.
        /// </summary>
        /// <remarks>
        /// GET /api/products/{id}/quantity  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("{id}/quantity")]
        public async Task<IActionResult> GetProductQuantity(int id)
        {
            var dto = await _productService.GetQuantityAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}
