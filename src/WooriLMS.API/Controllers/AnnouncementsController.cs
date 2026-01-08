using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;

    public AnnouncementsController(IAnnouncementService announcementService)
    {
        _announcementService = announcementService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetAllAnnouncements([FromQuery] bool includeUnpublished = false)
    {
        var isAdmin = User.IsInRole("Admin");
        var announcements = await _announcementService.GetAllAnnouncementsAsync(isAdmin && includeUnpublished);
        return Ok(announcements);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnnouncementDto>> GetAnnouncement(int id)
    {
        var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
        if (announcement == null)
            return NotFound();

        return Ok(announcement);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<AnnouncementDto>> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var announcement = await _announcementService.CreateAnnouncementAsync(userId, dto);
        return CreatedAtAction(nameof(GetAnnouncement), new { id = announcement.Id }, announcement);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<AnnouncementDto>> UpdateAnnouncement(int id, [FromBody] UpdateAnnouncementDto dto)
    {
        var announcement = await _announcementService.UpdateAnnouncementAsync(id, dto);
        if (announcement == null)
            return NotFound();

        return Ok(announcement);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAnnouncement(int id)
    {
        var result = await _announcementService.DeleteAnnouncementAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
