using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface IColorService
    {
        Task<IEnumerable<Color>> GetAllAsync();
        Task<Color?> GetByIdAsync(int id);
        Task<Color> CreateAsync(Color color);
        Task UpdateAsync(Color color);
        Task DeleteAsync(int id);
    }
}
