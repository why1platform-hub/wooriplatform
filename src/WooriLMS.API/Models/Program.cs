namespace WooriLMS.API.Models;

public class SkillProgram
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProgramCourse> ProgramCourses { get; set; } = new List<ProgramCourse>();
    public virtual ICollection<ProgramApplication> Applications { get; set; } = new List<ProgramApplication>();
}

public class ProgramCourse
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public int CourseId { get; set; }
    public int OrderIndex { get; set; }

    public virtual SkillProgram Program { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}

public class ProgramApplication
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string? ReviewNotes { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public virtual SkillProgram Program { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? SalaryRange { get; set; }
    public string JobType { get; set; } = "Full-time"; // Full-time, Part-time, Contract, Remote
    public string? RequiredSkills { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}

public class JobApplication
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string? ReviewNotes { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public virtual Job Job { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

public enum ApplicationStatus
{
    Pending = 0,
    UnderReview = 1,
    Accepted = 2,
    Rejected = 3,
    Withdrawn = 4
}
