namespace WooriLMS.API.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementType Type { get; set; } = AnnouncementType.General;
    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;
    public bool IsPublished { get; set; } = true;
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ApplicationUser CreatedBy { get; set; } = null!;
}

public enum AnnouncementType
{
    General = 0,
    Course = 1,
    Program = 2,
    Job = 3,
    System = 4
}

public enum AnnouncementPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
