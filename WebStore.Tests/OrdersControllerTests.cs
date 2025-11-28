using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebStore.Controllers;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Services.Interfaces;
using Xunit;

namespace WebStore.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;

        public OrdersControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
        }

        private OrdersController CreateControllerWithUser(ClaimsPrincipal? user = null)
        {
            var controller = new OrdersController(_orderServiceMock.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            return controller;
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithOrders()
        {
            var orders = new List<Order>
            {
                new Order { Id = 1 },
                new Order { Id = 2 }
            };

            _orderServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(orders);

            var controller = CreateControllerWithUser();

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count());
        }


        [Fact]
        public async Task GetById_ReturnsOk_WhenOrderExists()
        {

            var order = new Order { Id = 5 };

            _orderServiceMock
                .Setup(s => s.GetByIdAsync(5))
                .ReturnsAsync(order);

            var controller = CreateControllerWithUser();
            var result = await controller.GetById(5);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(5, returnedOrder.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            _orderServiceMock
                .Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((Order?)null);

            var controller = CreateControllerWithUser();

            var result = await controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenClientIdClaimMissing()
        {

            var identity = new ClaimsIdentity(); 
            var user = new ClaimsPrincipal(identity);

            var controller = CreateControllerWithUser(user);

            var request = new CreateOrderRequest
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 2 }
                }
            };

            var result = await controller.Create(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            _orderServiceMock.Verify(
                s => s.CreateAsync(It.IsAny<int>(), It.IsAny<List<OrderItemDto>>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenItemsNullOrEmpty()
        {
            var claims = new List<Claim>
            {
                new Claim("client_id", "10")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var controller = CreateControllerWithUser(user);

            var request = new CreateOrderRequest
            {
                Items = new List<OrderItemDto>()
            };

            var result = await controller.Create(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            _orderServiceMock.Verify(
                s => s.CreateAsync(It.IsAny<int>(), It.IsAny<List<OrderItemDto>>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAt_WhenValid()
        {

            var claims = new List<Claim>
            {
                new Claim("client_id", "10")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var controller = CreateControllerWithUser(user);

            var requestItems = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2 }
            };

            var request = new CreateOrderRequest
            {
                Items = requestItems
            };

            var createdOrder = new Order { Id = 123, ClientId = 10 };

            _orderServiceMock
                .Setup(s => s.CreateAsync(10, requestItems))
                .ReturnsAsync(createdOrder);


            var result = await controller.Create(request);


            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(OrdersController.GetById), created.ActionName);
            Assert.Equal(123, created.RouteValues["id"]);

            var returnedOrder = Assert.IsType<Order>(created.Value);
            Assert.Equal(123, returnedOrder.Id);
            Assert.Equal(10, returnedOrder.ClientId);

            _orderServiceMock.Verify(
                s => s.CreateAsync(10, requestItems),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_CallsService_AndReturnsNoContent()
        {
   
            var controller = CreateControllerWithUser();
            var request = new UpdateOrderStatusRequest
            {
                Status = "Confirmed"
            };

            var result = await controller.UpdateStatus(5, request);

            Assert.IsType<NoContentResult>(result);
            _orderServiceMock.Verify(
                s => s.UpdateStatusAsync(5, "Confirmed"),
                Times.Once);
        }
    }
}
