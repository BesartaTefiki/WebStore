using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    /// <summary>
    /// Manages product categories.
    /// </summary>
    /// <remarks>
    /// Base route: /api/categories
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Returns all categories.
        /// </summary>
        /// <remarks>
        /// GET /api/categories  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <remarks>
        /// POST /api/categories  
        /// Allowed roles: admin, advanced, simple.
        /// </remarks>
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            var created = await _categoryService.CreateAsync(category);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
