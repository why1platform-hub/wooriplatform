using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IDiscussionService
{
    Task<List<DiscussionDto>> GetAllDiscussionsAsync(string? category = null);
    Task<DiscussionDto?> GetDiscussionByIdAsync(int id);
    Task<DiscussionDto> CreateDiscussionAsync(string userId, CreateDiscussionDto dto);
    Task<DiscussionDto?> UpdateDiscussionAsync(int id, string userId, UpdateDiscussionDto dto, bool isAdmin = false);
    Task<bool> DeleteDiscussionAsync(int id, string userId, bool isAdmin = false);

    Task<DiscussionReplyDto> CreateReplyAsync(int discussionId, string userId, CreateReplyDto dto);
    Task<bool> DeleteReplyAsync(int replyId, string userId, bool isAdmin = false);
    Task<bool> MarkAsAcceptedAnswerAsync(int replyId, string discussionOwnerId);
}
