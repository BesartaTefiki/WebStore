using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services;
using Xunit;

namespace WebStore.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IClientRepository> _clientRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _clientRepoMock = new Mock<IClientRepository>();
            _productRepoMock = new Mock<IProductRepository>();

            _service = new OrderService(
                _orderRepoMock.Object,
                _clientRepoMock.Object,
                _productRepoMock.Object);
        }

    
        [Fact]
        public async Task GetAllAsync_ReturnsAllOrders()
        {
          
            var orders = new List<Order>
            {
                new Order { Id = 1 },
                new Order { Id = 2 }
            };

            _orderRepoMock
                .Setup(r => r.GetAllWithDetailsAsync())
                .ReturnsAsync(orders);

          
            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

      
        [Fact]
        public async Task GetByIdAsync_ReturnsOrder_WhenExists()
        {
           
            var order = new Order { Id = 5 };

            _orderRepoMock
                .Setup(r => r.GetByIdWithDetailsAsync(5))
                .ReturnsAsync(order);

            var result = await _service.GetByIdAsync(5);

          
            Assert.NotNull(result);
            Assert.Equal(5, result!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
      
            _orderRepoMock
                .Setup(r => r.GetByIdWithDetailsAsync(99))
                .ReturnsAsync((Order?)null);


            var result = await _service.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenClientDoesNotExist()
        {
            int clientId = 10;
            var items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2 }
            };

            _clientRepoMock
                .Setup(r => r.GetByIdAsync(clientId))
                .ReturnsAsync((Client?)null);


            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(clientId, items));

            _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenProductDoesNotExist()
        {

            int clientId = 10;
            var items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2 }
            };

            _clientRepoMock
                .Setup(r => r.GetByIdAsync(clientId))
                .ReturnsAsync(new Client { Id = clientId });

            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(clientId, items));

            _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_CreatesOrder_WithPendingStatus_AndItems()
        {
            int clientId = 10;
            var items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 2 },
                new OrderItemDto { ProductId = 2, Quantity = 3 }
            };

            _clientRepoMock
                .Setup(r => r.GetByIdAsync(clientId))
                .ReturnsAsync(new Client { Id = clientId });
        }
    }
}
