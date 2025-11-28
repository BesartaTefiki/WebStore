using WebStore.Models;

namespace WebStore.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetAllProducts();

        Task<IEnumerable<Product>> GetOutOfStockProducts();

        Task<IEnumerable<Product>> GetProductsByColor(string color);
        Task<IEnumerable<Product>> GetProductsByBrand(string brand);
        Task<IEnumerable<Product>> GetProductsByCategory(string category);
        Task<IEnumerable<Product>> GetProductsByGender(string gender);
        Task<IEnumerable<Product>> GetProductsBySize(string size);
        Task<IEnumerable<Product>> FindByPriceRange(decimal minPrice, decimal maxPrice);

        Task<IEnumerable<Product>> SearchAsync(
            int? categoryId,
            int? genderId,
            int? brandId,
            decimal? priceMin,
            decimal? priceMax,
            int? sizeId,
            int? colorId,
            bool? inStock
        );

        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int productId);
    }
}
