using System.ComponentModel.DataAnnotations;

namespace WooriLMS.API.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string InstructorId { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public int ModuleCount { get; set; }
    public int LessonCount { get; set; }
    public List<CourseModuleDto> Modules { get; set; } = new();
}

public class CourseModuleDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public List<LessonDto> Lessons { get; set; } = new();
}

public class LessonDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int OrderIndex { get; set; }
    public bool IsCompleted { get; set; }
    public int WatchedSeconds { get; set; }
}

public class CreateCourseDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    public string Level { get; set; } = "Beginner";
}

public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public string? Level { get; set; }
    public bool? IsPublished { get; set; }
}

public class CreateModuleDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public class CreateLessonDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string VideoUrl { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }
    public int OrderIndex { get; set; }
}

public class EnrollmentDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ProgressPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateProgressDto
{
    [Required]
    public int LessonId { get; set; }

    public int WatchedSeconds { get; set; }
    public bool IsCompleted { get; set; }
}
