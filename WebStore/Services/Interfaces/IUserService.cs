using WebStore.DTOs;

namespace WebStore.Services.Interfaces
{
    public interface IUserService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> CreateUserAsync(CreateUserAdminRequestDto request);
        Task<UserDto?> UpdateRoleAsync(int id, UpdateUserRoleRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}
