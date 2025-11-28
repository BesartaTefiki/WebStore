using WebStore.DTOs;

namespace WebStore.Services.Interfaces
{
    public interface IUserService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request);
    }
}
