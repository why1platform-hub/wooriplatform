using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Data;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class DiscussionService : IDiscussionService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DiscussionService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<DiscussionDto>> GetAllDiscussionsAsync(string? category = null)
    {
        var query = _context.Discussions
            .Include(d => d.User)
            .Include(d => d.Replies)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(d => d.Category == category);

        var discussions = await query
            .OrderByDescending(d => d.IsPinned)
            .ThenByDescending(d => d.CreatedAt)
            .ToListAsync();

        return discussions.Select(d => MapToDiscussionDto(d, false)).ToList();
    }

    public async Task<DiscussionDto?> GetDiscussionByIdAsync(int id)
    {
        var discussion = await _context.Discussions
            .Include(d => d.User)
            .Include(d => d.Replies)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (discussion == null) return null;

        discussion.ViewCount++;
        await _context.SaveChangesAsync();

        return await MapToDiscussionDtoWithRolesAsync(discussion);
    }

    public async Task<DiscussionDto> CreateDiscussionAsync(string userId, CreateDiscussionDto dto)
    {
        var discussion = new Discussion
        {
            UserId = userId,
            Title = dto.Title,
            Content = dto.Content,
            Category = dto.Category
        };

        _context.Discussions.Add(discussion);
        await _context.SaveChangesAsync();

        discussion = await _context.Discussions
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == discussion.Id);

        return MapToDiscussionDto(discussion!, false);
    }

    public async Task<DiscussionDto?> UpdateDiscussionAsync(int id, string userId, UpdateDiscussionDto dto, bool isAdmin = false)
    {
        var discussion = await _context.Discussions
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (discussion == null) return null;

        if (discussion.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You can only edit your own discussions");

        if (dto.Title != null) discussion.Title = dto.Title;
        if (dto.Content != null) discussion.Content = dto.Content;
        if (dto.Category != null) discussion.Category = dto.Category;
        if (dto.IsPinned.HasValue && isAdmin) discussion.IsPinned = dto.IsPinned.Value;
        if (dto.IsClosed.HasValue) discussion.IsClosed = dto.IsClosed.Value;

        discussion.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDiscussionDto(discussion, false);
    }

    public async Task<bool> DeleteDiscussionAsync(int id, string userId, bool isAdmin = false)
    {
        var discussion = await _context.Discussions.FindAsync(id);
        if (discussion == null) return false;

        if (discussion.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You can only delete your own discussions");

        _context.Discussions.Remove(discussion);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DiscussionReplyDto> CreateReplyAsync(int discussionId, string userId, CreateReplyDto dto)
    {
        var discussion = await _context.Discussions.FindAsync(discussionId);
        if (discussion == null)
            throw new InvalidOperationException("Discussion not found");

        if (discussion.IsClosed)
            throw new InvalidOperationException("This discussion is closed");

        var reply = new DiscussionReply
        {
            DiscussionId = discussionId,
            UserId = userId,
            Content = dto.Content
        };

        _context.DiscussionReplies.Add(reply);
        await _context.SaveChangesAsync();

        reply = await _context.DiscussionReplies
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reply.Id);

        var roles = await _userManager.GetRolesAsync(reply!.User);
        return MapToReplyDto(reply, roles.FirstOrDefault() ?? "Normal");
    }

    public async Task<bool> DeleteReplyAsync(int replyId, string userId, bool isAdmin = false)
    {
        var reply = await _context.DiscussionReplies.FindAsync(replyId);
        if (reply == null) return false;

        if (reply.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You can only delete your own replies");

        _context.DiscussionReplies.Remove(reply);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsAcceptedAnswerAsync(int replyId, string discussionOwnerId)
    {
        var reply = await _context.DiscussionReplies
            .Include(r => r.Discussion)
            .FirstOrDefaultAsync(r => r.Id == replyId);

        if (reply == null) return false;

        if (reply.Discussion.UserId != discussionOwnerId)
            throw new UnauthorizedAccessException("Only the discussion owner can mark an accepted answer");

        // Remove previous accepted answer
        var previousAccepted = await _context.DiscussionReplies
            .Where(r => r.DiscussionId == reply.DiscussionId && r.IsAcceptedAnswer)
            .ToListAsync();

        foreach (var prev in previousAccepted)
            prev.IsAcceptedAnswer = false;

        reply.IsAcceptedAnswer = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private static DiscussionDto MapToDiscussionDto(Discussion discussion, bool includeReplies)
    {
        return new DiscussionDto
        {
            Id = discussion.Id,
            UserId = discussion.UserId,
            UserName = $"{discussion.User?.FirstName} {discussion.User?.LastName}",
            UserProfileImage = discussion.User?.ProfileImageUrl,
            Title = discussion.Title,
            Content = discussion.Content,
            Category = discussion.Category,
            ViewCount = discussion.ViewCount,
            ReplyCount = discussion.Replies?.Count ?? 0,
            IsPinned = discussion.IsPinned,
            IsClosed = discussion.IsClosed,
            CreatedAt = discussion.CreatedAt,
            UpdatedAt = discussion.UpdatedAt
        };
    }

    private async Task<DiscussionDto> MapToDiscussionDtoWithRolesAsync(Discussion discussion)
    {
        var dto = MapToDiscussionDto(discussion, true);

        if (discussion.Replies != null)
        {
            dto.Replies = new List<DiscussionReplyDto>();

            foreach (var reply in discussion.Replies.OrderBy(r => r.CreatedAt))
            {
                var roles = await _userManager.GetRolesAsync(reply.User);
                dto.Replies.Add(MapToReplyDto(reply, roles.FirstOrDefault() ?? "Normal"));
            }
        }

        return dto;
    }

    private static DiscussionReplyDto MapToReplyDto(DiscussionReply reply, string role)
    {
        return new DiscussionReplyDto
        {
            Id = reply.Id,
            DiscussionId = reply.DiscussionId,
            UserId = reply.UserId,
            UserName = $"{reply.User?.FirstName} {reply.User?.LastName}",
            UserProfileImage = reply.User?.ProfileImageUrl,
            UserRole = role,
            Content = reply.Content,
            IsAcceptedAnswer = reply.IsAcceptedAnswer,
            CreatedAt = reply.CreatedAt,
            UpdatedAt = reply.UpdatedAt
        };
    }
}
