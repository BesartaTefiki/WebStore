using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Size>>> GetAll()
        {
            var sizes = await _sizeService.GetAllAsync();
            return Ok(sizes);
        }

        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Size>> Create(Size size)
        {
            var created = await _sizeService.CreateAsync(size);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
