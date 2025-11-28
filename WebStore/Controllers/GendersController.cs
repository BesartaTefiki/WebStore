using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    /// <summary>
    /// Manages gender categories for products (e.g. Women, Men, Kids).
    /// </summary>
    /// <remarks>
    /// Base route: /api/genders
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class GendersController : ControllerBase
    {
        private readonly IGenderService _genderService;

        public GendersController(IGenderService genderService)
        {
            _genderService = genderService;
        }

        /// <summary>
        /// Returns all genders.
        /// </summary>
        /// <remarks>
        /// GET /api/genders  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gender>>> GetAll()
        {
            var genders = await _genderService.GetAllAsync();
            return Ok(genders);
        }

        /// <summary>
        /// Creates a new gender.
        /// </summary>
        /// <remarks>
        /// POST /api/genders  
        /// Allowed roles: admin, advanced, simple.
        /// </remarks>
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Gender>> Create(Gender gender)
        {
            var created = await _genderService.CreateAsync(gender);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }

        /// <summary>
        /// Deletes a gender.
        /// </summary>
        /// <remarks>
        /// DELETE /api/genders/{id}  
        /// Allowed roles: admin, advanced, simple.
        /// </remarks>
        [Authorize(Roles = "admin,advanced,simple")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _genderService.DeleteAsync(id);
            return NoContent();
        }
    }
}
