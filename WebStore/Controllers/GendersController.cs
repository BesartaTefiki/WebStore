using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GendersController : ControllerBase
    {
        private readonly IGenderService _genderService;

        public GendersController(IGenderService genderService)
        {
            _genderService = genderService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gender>>> GetAll()
        {
            var genders = await _genderService.GetAllAsync();
            return Ok(genders);
        }

        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Gender>> Create(Gender gender)
        {
            var created = await _genderService.CreateAsync(gender);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }


        [Authorize(Roles = "admin,advanced,simple")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _genderService.DeleteAsync(id);
            return NoContent();
        }

    }
}
