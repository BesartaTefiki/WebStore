using WebStore.Models;

namespace WebStore.Repositories.Interfaces
{
    public interface ISizeRepository
    {
        Task<IEnumerable<Size>> GetAllAsync();
        Task<Size?> GetByIdAsync(int id);
        Task AddAsync(Size size);
        Task UpdateAsync(Size size);
        Task DeleteAsync(int id);
    }
}
