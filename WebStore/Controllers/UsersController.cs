using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.DTOs;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    /// <summary>
    /// Manages application users (list, details, create, change role, delete).
    /// </summary>
    /// <remarks>
    /// Base route: /api/users  
    /// Only users with the <c>admin</c> role can access these endpoints.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] // only admin can manage users
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <remarks>
        /// GET /api/users  
        /// Admin-only endpoint. Returns the list of all users with their roles.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Returns a single user by ID.
        /// </summary>
        /// <remarks>
        /// GET /api/users/{id}  
        /// Admin-only endpoint. Returns 404 if the user does not exist.
        /// </remarks>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user (staff or client) by admin.
        /// </summary>
        /// <remarks>
        /// POST /api/users  
        /// Admin-only endpoint.  
        /// Used to create staff accounts (admin/advanced/simple) or client users.
        /// Returns 400 if the operation is not valid (e.g. duplicate username).
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(
            [FromBody] CreateUserAdminRequestDto request)
        {
            try
            {
                var created = await _userService.CreateUserAsync(request);
                if (created == null) return BadRequest();
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the role (and type) of an existing user.
        /// </summary>
        /// <remarks>
        /// PUT /api/users/{id}/role  
        /// Admin-only endpoint.  
        /// Used to promote or demote users (e.g. from simple to advanced/admin).
        /// Returns 404 if the user does not exist.
        /// </remarks>
        [HttpPut("{id:int}/role")]
        public async Task<ActionResult<UserDto>> UpdateRole(
            int id,
            [FromBody] UpdateUserRoleRequestDto request)
        {
            try
            {
                var updated = await _userService.UpdateRoleAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <remarks>
        /// DELETE /api/users/{id}  
        /// Admin-only endpoint.  
        /// Returns 204 on success, 404 if the user does not exist.
        /// </remarks>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
