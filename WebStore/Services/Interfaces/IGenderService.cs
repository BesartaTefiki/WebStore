using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface IGenderService
    {
        Task<IEnumerable<Gender>> GetAllAsync();
        Task<Gender?> GetByIdAsync(int id);
        Task<Gender> CreateAsync(Gender gender);
        Task UpdateAsync(Gender gender);
        Task DeleteAsync(int id);
    }
}
