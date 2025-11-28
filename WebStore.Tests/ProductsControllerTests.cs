using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebStore.Controllers;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Services.Interfaces;
using Xunit;

namespace WebStore.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _serviceMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _serviceMock = new Mock<IProductService>();
            _controller = new ProductsController(_serviceMock.Object);
        }

  
        [Fact]
        public async Task GetProducts_ReturnsOkWithProducts()
        {
          
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "P1" },
                new Product { Id = 2, Name = "P2" }
            };

            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(products);

          
            var result = await _controller.GetProducts();

        
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
        }

     
        [Fact]
        public async Task GetProduct_ReturnsOk_WhenProductExists()
        {
         
            var product = new Product { Id = 1, Name = "Test" };

            _serviceMock
                .Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(product);

          
            var result = await _controller.GetProduct(1);

          
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
         
            _serviceMock
                .Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((Product?)null);

          
            var result = await _controller.GetProduct(99);

      
            Assert.IsType<NotFoundResult>(result.Result);
        }

       
        [Fact]
        public async Task CreateProduct_ReturnsCreatedAt_WithCreatedProduct()
        {
            
            var productToCreate = new Product { Id = 5, Name = "New" };

            _serviceMock
                .Setup(s => s.CreateAsync(productToCreate))
                .ReturnsAsync(productToCreate);

       
            var result = await _controller.CreateProduct(productToCreate);

        
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ProductsController.GetProduct), createdResult.ActionName);

            Assert.Equal(productToCreate.Id, createdResult.RouteValues["id"]);
            var returnedProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal(5, returnedProduct.Id);

            _serviceMock.Verify(s => s.CreateAsync(productToCreate), Times.Once);
        }

      
        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            
            var product = new Product { Id = 2, Name = "X" };

            var result = await _controller.UpdateProduct(1, product);

   
            Assert.IsType<BadRequestResult>(result);
            _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenValid()
        {
          
            var product = new Product { Id = 1, Name = "Updated" };

          
            var result = await _controller.UpdateProduct(1, product);

        
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.UpdateAsync(product), Times.Once);
        }


        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_AndCallsService()
        {
            var result = await _controller.DeleteProduct(3);

            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.DeleteAsync(3), Times.Once);
        }


        [Fact]
        public async Task SetDiscount_CallsService_AndReturnsNoContent()
        {
          
            var dto = new DiscountRequestDto { DiscountPercent = 15m };

            var result = await _controller.SetDiscount(10, dto);

    
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(
                s => s.ApplyDiscountAsync(10, 15m),
                Times.Once);
        }

        [Fact]
        public async Task SearchProducts_ReturnsOkWithResult()
        {

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Filtered" }
            };

            _serviceMock
                .Setup(s => s.SearchAsync(
                    1, 2, 3,
                    10m, 50m,
                    4, 5,
                    true))
                .ReturnsAsync(products);

            var result = await _controller.SearchProducts(
                categoryId: 1,
                genderId: 2,
                brandId: 3,
                priceMin: 10m,
                priceMax: 50m,
                sizeId: 4,
                colorId: 5,
                inStock: true);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            var single = Assert.Single(returnedProducts);
            Assert.Equal(1, single.Id);
        }


        [Fact]
        public async Task GetProductQuantity_ReturnsNotFound_WhenDtoIsNull()
        {

            _serviceMock
                .Setup(s => s.GetQuantityAsync(7))
                .ReturnsAsync((ProductQuantityDto?)null);

            var result = await _controller.GetProductQuantity(7);

       
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProductQuantity_ReturnsOkWithDto_WhenExists()
        {
 
            var dto = new ProductQuantityDto
            {
                ProductId = 7,
                Name = "Shirt",
                InitialQuantity = 100,
                SoldQuantity = 20,
                CurrentQuantity = 80
            };

            _serviceMock
                .Setup(s => s.GetQuantityAsync(7))
                .ReturnsAsync(dto);

            var result = await _controller.GetProductQuantity(7);

      
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<ProductQuantityDto>(okResult.Value);
            Assert.Equal(7, returnedDto.ProductId);
        }
    }
}
