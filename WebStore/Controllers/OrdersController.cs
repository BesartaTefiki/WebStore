using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Returns all orders.
        /// </summary>
        /// <remarks>
        /// GET /api/orders  
        /// Roles allowed: admin, advanced.
        /// </remarks>
        [Authorize(Roles = "admin,advanced")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Returns order by id.
        /// </summary>
        /// <remarks>
        /// GET /api/orders/{id}  
        /// Roles allowed: admin, advanced.
        /// </remarks>
        [Authorize(Roles = "admin,advanced")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        /// <summary>
        /// Creates a new order for the logged-in client.
        /// </summary>
        /// <remarks>
        /// POST /api/orders  
        /// Requires authenticated user with client_id claim.
        /// </remarks>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> Create(CreateOrderRequest request)
        {
            var clientClaim = User.FindFirst("client_id")?.Value;
            if (string.IsNullOrEmpty(clientClaim))
                return BadRequest(new { message = "User is not linked to a client." });

            var order = await _orderService.CreateAsync(int.Parse(clientClaim), request.Items);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        /// <summary>
        /// Updates order status.
        /// </summary>
        /// <remarks>
        /// PUT /api/orders/{id}/status  
        /// Roles allowed: admin, advanced.
        /// </remarks>
        [Authorize(Roles = "admin,advanced")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request)
        {
            await _orderService.UpdateStatusAsync(id, request.Status);
            return NoContent();
        }
    }
}
