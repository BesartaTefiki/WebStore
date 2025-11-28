// Controllers/OrdersController.cs
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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

        // GET: api/orders  (admin + advanced)
        [Authorize(Roles = "admin,advanced")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        // GET: api/orders/5  (admin + advanced)
        [Authorize(Roles = "admin,advanced")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST: api/orders  (any logged-in user – client comes from JWT)
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> Create(CreateOrderRequest request)
        {
            
            var clientIdClaim = User.FindFirst("client_id")?.Value;
            if (string.IsNullOrEmpty(clientIdClaim))
            {
                return BadRequest(new
                {
                    message = "Logged-in user is not linked to a client."
                });
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Order must contain at least one item."
                });
            }

            int clientId = int.Parse(clientIdClaim);

            var order = await _orderService.CreateAsync(clientId, request.Items);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        // PUT: api/orders/5/status  (admin + advanced)
        [Authorize(Roles = "admin,advanced")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request)
        {
            await _orderService.UpdateStatusAsync(id, request.Status);
            return NoContent();
        }
    }
}
