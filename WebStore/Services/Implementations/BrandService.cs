using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
            => await _brandRepository.GetAllAsync();

        public async Task<Brand?> GetByIdAsync(int id)
            => await _brandRepository.GetByIdAsync(id);

        public async Task<Brand> CreateAsync(Brand brand)
        {
            await _brandRepository.AddAsync(brand);
            return brand;
        }

        public async Task UpdateAsync(Brand brand)
        {
            await _brandRepository.UpdateAsync(brand);
        }

        public async Task DeleteAsync(int id)
        {
            await _brandRepository.DeleteAsync(id);
        }
    }
}
