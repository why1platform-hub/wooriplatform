using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    string GenerateJwtToken(string userId, string email, string role);
}
