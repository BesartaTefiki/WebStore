using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface ISizeService
    {
        Task<IEnumerable<Size>> GetAllAsync();
        Task<Size?> GetByIdAsync(int id);
        Task<Size> CreateAsync(Size size);
        Task UpdateAsync(Size size);
        Task DeleteAsync(int id);
    }
}
