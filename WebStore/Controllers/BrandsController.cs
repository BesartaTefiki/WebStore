using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [Authorize(Roles = "admin,advanced,simple")]
        [HttpPost]
        public async Task<ActionResult<Brand>> Create(Brand brand)
        {
            var created = await _brandService.CreateAsync(brand);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
