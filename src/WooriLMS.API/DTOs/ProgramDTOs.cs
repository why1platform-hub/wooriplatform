using System.ComponentModel.DataAnnotations;

namespace WooriLMS.API.DTOs;

public class SkillProgramDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Industry { get; set; } = string.Empty;
    public int DurationWeeks { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProgramCourseDto> Courses { get; set; } = new();
}

public class ProgramCourseDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}

public class CreateProgramDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [Required]
    public string Industry { get; set; } = string.Empty;

    public int DurationWeeks { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public int MaxParticipants { get; set; } = 50;
    public List<int>? CourseIds { get; set; }
}

public class UpdateProgramDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Industry { get; set; }
    public int? DurationWeeks { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxParticipants { get; set; }
    public bool? IsActive { get; set; }
}

public class ProgramApplicationDto
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public string ProgramTitle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string? CoverLetter { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class CreateProgramApplicationDto
{
    [Required]
    public int ProgramId { get; set; }

    public string? CoverLetter { get; set; }
}

public class UpdateApplicationStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    public string? ReviewNotes { get; set; }
}

public class JobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? SalaryRange { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string? RequiredSkills { get; set; }
    public bool IsActive { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int ApplicationCount { get; set; }
}

public class CreateJobDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;

    public string? SalaryRange { get; set; }
    public string JobType { get; set; } = "Full-time";
    public string? RequiredSkills { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UpdateJobDto
{
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public string? JobType { get; set; }
    public string? RequiredSkills { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class JobApplicationDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class CreateJobApplicationDto
{
    [Required]
    public int JobId { get; set; }

    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
}
