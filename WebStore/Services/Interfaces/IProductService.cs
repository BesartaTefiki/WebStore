using WebStore.DTOs;
using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

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

        Task<ProductQuantityDto?> GetQuantityAsync(int productId);
        Task ApplyDiscountAsync(int productId, decimal discountPercent);
        Task RemoveDiscountAsync(int productId);
    }
}
