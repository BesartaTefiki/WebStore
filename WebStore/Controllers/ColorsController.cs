using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Color>>> GetAll()
        {
            var colors = await _colorService.GetAllAsync();
            return Ok(colors);
        }

        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Color>> Create(Color color)
        {
            var created = await _colorService.CreateAsync(color);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
