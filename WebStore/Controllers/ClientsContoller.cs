using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    /// <summary>
    /// Manages clients (customers) in the system.
    /// </summary>
    /// <remarks>
    /// Base route: /api/clients  
    /// Protected by roles: admin, advanced.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,advanced")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Returns all clients.
        /// </summary>
        /// <remarks>
        /// GET /api/clients  
        /// Only admin and advanced users can access this.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(clients);
        }

        /// <summary>
        /// Returns a client by ID.
        /// </summary>
        /// <remarks>
        /// GET /api/clients/{id}
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return Ok(client);
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <remarks>
        /// POST /api/clients
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Client>> Create(Client client)
        {
            var created = await _clientService.CreateAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing client.
        /// </summary>
        /// <remarks>
        /// PUT /api/clients/{id}
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Client client)
        {
            if (id != client.Id) return BadRequest();

            await _clientService.UpdateAsync(client);
            return NoContent();
        }

        /// <summary>
        /// Deletes a client.
        /// </summary>
        /// <remarks>
        /// DELETE /api/clients/{id}
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _clientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
