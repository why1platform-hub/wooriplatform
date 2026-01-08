using System.ComponentModel.DataAnnotations;

namespace WooriLMS.API.DTOs;

public class AnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string CreatedById { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateAnnouncementDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string Type { get; set; } = "General";
    public string Priority { get; set; } = "Normal";
    public bool IsPublished { get; set; } = true;
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class UpdateAnnouncementDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public string? Priority { get; set; }
    public bool? IsPublished { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
