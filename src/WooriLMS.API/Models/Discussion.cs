namespace WooriLMS.API.Models;

public class Discussion
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int ViewCount { get; set; } = 0;
    public bool IsPinned { get; set; } = false;
    public bool IsClosed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<DiscussionReply> Replies { get; set; } = new List<DiscussionReply>();
}

public class DiscussionReply
{
    public int Id { get; set; }
    public int DiscussionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsAcceptedAnswer { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Discussion Discussion { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

public class FAQ
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int OrderIndex { get; set; } = 0;
    public bool IsPublished { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
