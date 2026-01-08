using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Models;

namespace WooriLMS.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseModule> CourseModules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<ConsultantTimeSlot> ConsultantTimeSlots { get; set; }
    public DbSet<ConsultantBooking> ConsultantBookings { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<DiscussionReply> DiscussionReplies { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<SkillProgram> SkillPrograms { get; set; }
    public DbSet<ProgramCourse> ProgramCourses { get; set; }
    public DbSet<ProgramApplication> ProgramApplications { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<Announcement> Announcements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Course relationships
        builder.Entity<Course>()
            .HasOne(c => c.Instructor)
            .WithMany(u => u.InstructedCourses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CourseModule>()
            .HasOne(m => m.Course)
            .WithMany(c => c.Modules)
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Lesson>()
            .HasOne(l => l.Module)
            .WithMany(m => m.Lessons)
            .HasForeignKey(l => l.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enrollment relationships
        builder.Entity<CourseEnrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CourseEnrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CourseEnrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        // Lesson Progress
        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.Lesson)
            .WithMany(l => l.LessonProgresses)
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LessonProgress>()
            .HasIndex(lp => new { lp.UserId, lp.LessonId })
            .IsUnique();

        // Consultant relationships
        builder.Entity<ConsultantTimeSlot>()
            .HasOne(ts => ts.Instructor)
            .WithMany(u => u.ConsultantTimeSlots)
            .HasForeignKey(ts => ts.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ConsultantBooking>()
            .HasOne(b => b.TimeSlot)
            .WithOne(ts => ts.Booking)
            .HasForeignKey<ConsultantBooking>(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ConsultantBooking>()
            .HasOne(b => b.User)
            .WithMany(u => u.ConsultantBookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Discussion relationships
        builder.Entity<Discussion>()
            .HasOne(d => d.User)
            .WithMany(u => u.Discussions)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DiscussionReply>()
            .HasOne(r => r.Discussion)
            .WithMany(d => d.Replies)
            .HasForeignKey(r => r.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DiscussionReply>()
            .HasOne(r => r.User)
            .WithMany(u => u.DiscussionReplies)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Program relationships
        builder.Entity<ProgramCourse>()
            .HasOne(pc => pc.Program)
            .WithMany(p => p.ProgramCourses)
            .HasForeignKey(pc => pc.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProgramCourse>()
            .HasOne(pc => pc.Course)
            .WithMany()
            .HasForeignKey(pc => pc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProgramApplication>()
            .HasOne(pa => pa.Program)
            .WithMany(p => p.Applications)
            .HasForeignKey(pa => pa.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProgramApplication>()
            .HasOne(pa => pa.User)
            .WithMany(u => u.ProgramApplications)
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProgramApplication>()
            .HasIndex(pa => new { pa.ProgramId, pa.UserId })
            .IsUnique();

        // Job relationships
        builder.Entity<JobApplication>()
            .HasOne(ja => ja.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobApplication>()
            .HasOne(ja => ja.User)
            .WithMany(u => u.JobApplications)
            .HasForeignKey(ja => ja.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobApplication>()
            .HasIndex(ja => new { ja.JobId, ja.UserId })
            .IsUnique();

        // Announcement relationships
        builder.Entity<Announcement>()
            .HasOne(a => a.CreatedBy)
            .WithMany()
            .HasForeignKey(a => a.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
