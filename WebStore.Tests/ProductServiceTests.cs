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
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _orderRepoMock = new Mock<IOrderRepository>();

            _service = new ProductService(
                _productRepoMock.Object,
                _orderRepoMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "P1" },
                new Product { Id = 2, Name = "P2" }
            };

            _productRepoMock
                .Setup(r => r.GetAllProducts())
                .ReturnsAsync(products);

            var result = await _service.GetAllAsync();


            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_WhenExists()
        {

            var product = new Product { Id = 1, Name = "Test" };

            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync(product);

            var result = await _service.GetByIdAsync(1);


            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {

            _productRepoMock
                .Setup(r => r.GetProductById(99))
                .ReturnsAsync((Product?)null);

            var result = await _service.GetByIdAsync(99);


            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_CallsAddProduct_AndReturnsSameInstance()
        {
          
            var product = new Product { Id = 1, Name = "New" };

            _productRepoMock
                .Setup(r => r.AddProduct(product))
                .Returns(Task.CompletedTask);


            var result = await _service.CreateAsync(product);


            _productRepoMock.Verify(r => r.AddProduct(product), Times.Once);
            Assert.Same(product, result); 
        }

        [Fact]
        public async Task UpdateAsync_CallsUpdateProduct()
        {
       
            var product = new Product { Id = 1, Name = "Updated" };

            await _service.UpdateAsync(product);
       
            _productRepoMock.Verify(r => r.UpdateProduct(product), Times.Once);
        }

      
        [Fact]
        public async Task DeleteAsync_CallsDeleteProduct()
        {
  
            var id = 5;

            await _service.DeleteAsync(id);

            _productRepoMock.Verify(r => r.DeleteProduct(id), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_ForwardsParametersToRepository()
        {

            var expected = new List<Product> { new Product { Id = 10, Name = "Found" } };

            _productRepoMock
                .Setup(r => r.SearchAsync(
                    1, 2, 3,
                    10m, 50m,
                    4, 5,
                    true))
                .ReturnsAsync(expected);


            var result = await _service.SearchAsync(
                categoryId: 1,
                genderId: 2,
                brandId: 3,
                priceMin: 10m,
                priceMax: 50m,
                sizeId: 4,
                colorId: 5,
                inStock: true);

            Assert.Single(result);
            Assert.Equal(10, result.First().Id);
        }

        [Fact]
        public async Task GetQuantityAsync_ReturnsNull_WhenProductDoesNotExist()
        {
     
            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync((Product?)null);

          
            var result = await _service.GetQuantityAsync(1);

   
            Assert.Null(result);
        }

        [Fact]
        public async Task GetQuantityAsync_ComputesQuantitiesCorrectly()
        {
       
            var product = new Product { Id = 1, Name = "Shirt", Quantity = 100 };

            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync(product);

            var orders = new List<Order>
            {
                new Order
                {
                    Status = "Confirmed",
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 10 },
                        new OrderItem { ProductId = 2, Quantity = 5 } 
                    }
                },
                new Order
                {
                    Status = "Pending",   
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 20 }
                    }
                },
                new Order
                {
                    Status = "Confirmed",
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 15 }
                    }
                }
            };

            _orderRepoMock
                .Setup(r => r.GetAllWithDetailsAsync())
                .ReturnsAsync(orders);

     
            var dto = await _service.GetQuantityAsync(1);

            Assert.NotNull(dto);
            Assert.Equal(1, dto!.ProductId);
            Assert.Equal(100, dto.InitialQuantity);

            Assert.Equal(25, dto.SoldQuantity);

       
            Assert.Equal(75, dto.CurrentQuantity);
        }

     
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public async Task ApplyDiscountAsync_Throws_WhenDiscountOutOfRange(decimal discount)
        {
        
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.ApplyDiscountAsync(1, discount));
        }

        [Fact]
        public async Task ApplyDiscountAsync_Throws_WhenProductNotFound()
        {

            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.ApplyDiscountAsync(1, 10m));
        }

        [Fact]
        public async Task ApplyDiscountAsync_SetsDiscount_AndUpdatesProduct()
        {
      
            var product = new Product { Id = 1, Name = "P", DiscountPercent = 0 };

            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync(product);

            await _service.ApplyDiscountAsync(1, 15m);

  
            Assert.Equal(15m, product.DiscountPercent);
            _productRepoMock.Verify(r => r.UpdateProduct(product), Times.Once);
        }

        [Fact]
        public async Task RemoveDiscountAsync_Throws_WhenProductNotFound()
        {
            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RemoveDiscountAsync(1));
        }

        [Fact]
        public async Task RemoveDiscountAsync_SetsDiscountToZero_AndUpdatesProduct()
        {
           
            var product = new Product { Id = 1, Name = "P", DiscountPercent = 30m };

            _productRepoMock
                .Setup(r => r.GetProductById(1))
                .ReturnsAsync(product);

            await _service.RemoveDiscountAsync(1);

            Assert.Equal(0m, product.DiscountPercent);
            _productRepoMock.Verify(r => r.UpdateProduct(product), Times.Once);
        }
    }
}
