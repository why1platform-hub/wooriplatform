using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(MapToUserDto(user, roles.FirstOrDefault() ?? "Normal"));
        }

        return userDtos.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToUserDto(user, roles.FirstOrDefault() ?? "Normal");
    }

    public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            userDtos.Add(MapToUserDto(user, role));
        }

        return userDtos.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
    }

    public async Task<UserDto?> UpdateUserAsync(string id, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
        if (dto.ProfileImageUrl != null) user.ProfileImageUrl = dto.ProfileImageUrl;
        if (dto.Bio != null) user.Bio = dto.Bio;
        if (dto.Skills != null) user.Skills = dto.Skills;
        if (dto.WorkExperience != null) user.WorkExperience = dto.WorkExperience;
        if (dto.Education != null) user.Education = dto.Education;
        if (dto.LinkedInUrl != null) user.LinkedInUrl = dto.LinkedInUrl;
        if (dto.ResumeUrl != null) user.ResumeUrl = dto.ResumeUrl;

        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        return MapToUserDto(user, roles.FirstOrDefault() ?? "Normal");
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, newRole);

        // Update UserType enum
        user.UserType = newRole switch
        {
            "Admin" => UserType.Admin,
            "Instructor" => UserType.Instructor,
            _ => UserType.Normal
        };

        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ToggleUserStatusAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    private static UserDto MapToUserDto(ApplicationUser user, string role)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImageUrl = user.ProfileImageUrl,
            Bio = user.Bio,
            Skills = user.Skills,
            WorkExperience = user.WorkExperience,
            Education = user.Education,
            LinkedInUrl = user.LinkedInUrl,
            ResumeUrl = user.ResumeUrl,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            UserType = user.UserType.ToString(),
            Role = role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
