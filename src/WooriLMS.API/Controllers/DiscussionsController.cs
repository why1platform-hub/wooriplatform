using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscussionsController : ControllerBase
{
    private readonly IDiscussionService _discussionService;

    public DiscussionsController(IDiscussionService discussionService)
    {
        _discussionService = discussionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DiscussionDto>>> GetAllDiscussions([FromQuery] string? category = null)
    {
        var discussions = await _discussionService.GetAllDiscussionsAsync(category);
        return Ok(discussions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiscussionDto>> GetDiscussion(int id)
    {
        var discussion = await _discussionService.GetDiscussionByIdAsync(id);
        if (discussion == null)
            return NotFound();

        return Ok(discussion);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DiscussionDto>> CreateDiscussion([FromBody] CreateDiscussionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var discussion = await _discussionService.CreateDiscussionAsync(userId, dto);
        return CreatedAtAction(nameof(GetDiscussion), new { id = discussion.Id }, discussion);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<DiscussionDto>> UpdateDiscussion(int id, [FromBody] UpdateDiscussionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var discussion = await _discussionService.UpdateDiscussionAsync(id, userId, dto, isAdmin);
            if (discussion == null)
                return NotFound();

            return Ok(discussion);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDiscussion(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var result = await _discussionService.DeleteDiscussionAsync(id, userId, isAdmin);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // Replies
    [Authorize]
    [HttpPost("{discussionId}/replies")]
    public async Task<ActionResult<DiscussionReplyDto>> CreateReply(int discussionId, [FromBody] CreateReplyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var reply = await _discussionService.CreateReplyAsync(discussionId, userId, dto);
            return Ok(reply);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("replies/{replyId}")]
    public async Task<ActionResult> DeleteReply(int replyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var result = await _discussionService.DeleteReplyAsync(replyId, userId, isAdmin);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("replies/{replyId}/accept")]
    public async Task<ActionResult> MarkAsAcceptedAnswer(int replyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _discussionService.MarkAsAcceptedAnswerAsync(replyId, userId);
            if (!result)
                return NotFound();

            return Ok(new { message = "Answer marked as accepted" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
