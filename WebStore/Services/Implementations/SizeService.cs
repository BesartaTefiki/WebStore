using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;

        public SizeService(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
        }

        public async Task<IEnumerable<Size>> GetAllAsync()
            => await _sizeRepository.GetAllAsync();

        public async Task<Size?> GetByIdAsync(int id)
            => await _sizeRepository.GetByIdAsync(id);

        public async Task<Size> CreateAsync(Size size)
        {
            await _sizeRepository.AddAsync(size);
            return size;
        }

        public async Task UpdateAsync(Size size)
        {
            await _sizeRepository.UpdateAsync(size);
        }

        public async Task DeleteAsync(int id)
        {
            await _sizeRepository.DeleteAsync(id);
        }
    }
}
