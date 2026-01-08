using Microsoft.AspNetCore.Identity;

namespace WooriLMS.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public string? WorkExperience { get; set; }
    public string? Education { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? ResumeUrl { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public UserType UserType { get; set; } = UserType.Normal;

    // Navigation properties
    public virtual ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    public virtual ICollection<Course> InstructedCourses { get; set; } = new List<Course>();
    public virtual ICollection<ConsultantBooking> ConsultantBookings { get; set; } = new List<ConsultantBooking>();
    public virtual ICollection<ConsultantTimeSlot> ConsultantTimeSlots { get; set; } = new List<ConsultantTimeSlot>();
    public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
    public virtual ICollection<DiscussionReply> DiscussionReplies { get; set; } = new List<DiscussionReply>();
    public virtual ICollection<ProgramApplication> ProgramApplications { get; set; } = new List<ProgramApplication>();
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}

public enum UserType
{
    Normal = 0,
    Instructor = 1,
    Admin = 2
}
