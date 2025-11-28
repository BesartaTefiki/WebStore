using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebStoreContext _context;

        public ProductRepository(WebStoreContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)  
                .Include(p => p.Colors) 
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)  
                .Include(p => p.Colors) 
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProducts()
        {
            return await _context.Products
                .Where(p => p.Quantity <= 0)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes) 
                .Include(p => p.Colors)  
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByColor(string color)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Where(p =>
                    p.Colors.Any(c => c.Name.ToLower() == color.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrand(string brand)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Where(p => p.Brand.Name.ToLower() == brand.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Where(p => p.Category.Name.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByGender(string gender)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Where(p => p.Gender.Name.ToLower() == gender.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsBySize(string size)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Where(p =>
                    p.Sizes.Any(s => s.Name.ToLower() == size.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> FindByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
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
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (genderId.HasValue)
                query = query.Where(p => p.GenderId == genderId.Value);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            if (sizeId.HasValue)
                query = query.Where(p =>
                    p.Sizes.Any(s => s.Id == sizeId.Value));

            if (colorId.HasValue)
                query = query.Where(p =>
                    p.Colors.Any(c => c.Id == colorId.Value));

            if (priceMin.HasValue)
                query = query.Where(p => p.Price >= priceMin.Value);

            if (priceMax.HasValue)
                query = query.Where(p => p.Price <= priceMax.Value);

            if (inStock == true)
                query = query.Where(p => p.Quantity > 0);

            return await query.ToListAsync();
        }

        public async Task AddProduct(Product product)
        {
            if (product.Sizes != null && product.Sizes.Count > 0)
            {
                foreach (var size in product.Sizes)
                {
                    _context.Attach(size);
                }
            }

            if (product.Colors != null && product.Colors.Count > 0)
            {
                foreach (var color in product.Colors)
                {
                    _context.Attach(color);
                }
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            var existing = await _context.Products.FindAsync(productId);
            if (existing != null)
            {
                _context.Products.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
