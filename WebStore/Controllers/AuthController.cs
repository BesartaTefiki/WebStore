using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.DTOs;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <remarks>
        /// POST /api/auth/register  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
        {
            var result = await _userService.RegisterAsync(request);

            if (result == null)
                return BadRequest(new { message = "Username already exists" });

            return Ok(result);
        }

        /// <summary>
        /// Authenticates user and returns JWT.
        /// </summary>
        /// <remarks>
        /// POST /api/auth/login  
        /// Public endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var response = await _userService.LoginAsync(request);

            if (response == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(response);
        }
    }
}
