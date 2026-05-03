using ScanPoint.Models.DTOs;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserResponseDto> RegisterAsync(RegisterUserDto dto);
        Task<LoginResponseDto> LoginAsync(LoginUserDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
    }
}
