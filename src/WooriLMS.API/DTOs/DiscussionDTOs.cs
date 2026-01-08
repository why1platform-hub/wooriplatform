using System.ComponentModel.DataAnnotations;

namespace WooriLMS.API.DTOs;

public class DiscussionDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserProfileImage { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int ReplyCount { get; set; }
    public bool IsPinned { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DiscussionReplyDto> Replies { get; set; } = new();
}

public class DiscussionReplyDto
{
    public int Id { get; set; }
    public int DiscussionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserProfileImage { get; set; }
    public string UserRole { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsAcceptedAnswer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateDiscussionDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string Category { get; set; } = "General";
}

public class UpdateDiscussionDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public bool? IsPinned { get; set; }
    public bool? IsClosed { get; set; }
}

public class CreateReplyDto
{
    [Required]
    public string Content { get; set; } = string.Empty;
}

public class FaqDto
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFaqDto
{
    [Required]
    public string Question { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;

    public string Category { get; set; } = "General";
    public int OrderIndex { get; set; } = 0;
    public bool IsPublished { get; set; } = true;
}

public class UpdateFaqDto
{
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public string? Category { get; set; }
    public int? OrderIndex { get; set; }
    public bool? IsPublished { get; set; }
}
