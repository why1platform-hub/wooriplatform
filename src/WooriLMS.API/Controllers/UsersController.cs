using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("role/{role}")]
    public async Task<ActionResult<List<UserDto>>> GetUsersByRole(string role)
    {
        var users = await _userService.GetUsersByRoleAsync(role);
        return Ok(users);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] UpdateProfileDto dto)
    {
        var user = await _userService.UpdateUserAsync(id, dto);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _userService.UpdateUserRoleAsync(id, dto.Role);
        if (!result)
            return NotFound();

        return Ok(new { message = "User role updated" });
    }

    [HttpPut("{id}/toggle-status")]
    public async Task<ActionResult> ToggleUserStatus(string id)
    {
        var result = await _userService.ToggleUserStatusAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "User status toggled" });
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult> ResetUserPassword(string id, [FromBody] ResetPasswordDto dto)
    {
        dto.UserId = id;
        var result = await _authService.ResetPasswordAsync(dto);
        if (!result)
            return BadRequest(new { message = "Failed to reset password" });

        return Ok(new { message = "Password reset successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

public class UpdateRoleDto
{
    public string Role { get; set; } = string.Empty;
}
