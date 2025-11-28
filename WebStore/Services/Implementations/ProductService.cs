using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductService(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllProducts();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetProductById(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productRepository.AddProduct(product);
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            await _productRepository.UpdateProduct(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteProduct(id);
        }

        public async Task<IEnumerable<Product>> SearchAsync(
            int? categoryId,
            int? genderId,
            int? brandId,
            decimal? priceMin,
            decimal? priceMax,
            int? sizeId,
            int? colorId,
            bool? inStock
        )
        {
            return await _productRepository.SearchAsync(
                categoryId,
                genderId,
                brandId,
                priceMin,
                priceMax,
                sizeId,
                colorId,
                inStock
            );
        }

        public async Task<ProductQuantityDto?> GetQuantityAsync(int productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null) return null;

            var orders = await _orderRepository.GetAllWithDetailsAsync();

            var soldQuantity = orders
                .Where(o => o.Status == "Confirmed")
                .SelectMany(o => o.Items)
                .Where(oi => oi.ProductId == productId)
                .Sum(oi => oi.Quantity);

            var currentQuantity = product.Quantity - soldQuantity;

            return new ProductQuantityDto
            {
                ProductId = product.Id,
                Name = product.Name,
                InitialQuantity = product.Quantity,
                SoldQuantity = soldQuantity,
                CurrentQuantity = currentQuantity
            };
        }

        public async Task ApplyDiscountAsync(int productId, decimal discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                throw new InvalidOperationException("Discount must be between 0 and 100.");

            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                throw new InvalidOperationException($"Product with id {productId} does not exist.");

            product.DiscountPercent = discountPercent;

            await _productRepository.UpdateProduct(product);
        }

        public async Task RemoveDiscountAsync(int productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                throw new InvalidOperationException($"Product with id {productId} does not exist.");

            product.DiscountPercent = 0;

            await _productRepository.UpdateProduct(product);
        }
    }
}
