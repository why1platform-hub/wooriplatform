using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<List<UserDto>> GetUsersByRoleAsync(string role);
    Task<UserDto?> UpdateUserAsync(string id, UpdateProfileDto dto);
    Task<bool> UpdateUserRoleAsync(string userId, string newRole);
    Task<bool> ToggleUserStatusAsync(string userId);
    Task<bool> DeleteUserAsync(string userId);
}
