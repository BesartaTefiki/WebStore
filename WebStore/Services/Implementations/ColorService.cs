using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;

        public ColorService(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public async Task<IEnumerable<Color>> GetAllAsync()
            => await _colorRepository.GetAllAsync();

        public async Task<Color?> GetByIdAsync(int id)
            => await _colorRepository.GetByIdAsync(id);

        public async Task<Color> CreateAsync(Color color)
        {
            await _colorRepository.AddAsync(color);
            return color;
        }

        public async Task UpdateAsync(Color color)
        {
            await _colorRepository.UpdateAsync(color);
        }

        public async Task DeleteAsync(int id)
        {
            await _colorRepository.DeleteAsync(id);
        }
    }
}
