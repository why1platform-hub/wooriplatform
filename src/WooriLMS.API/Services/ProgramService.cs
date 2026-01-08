using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Data;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class ProgramService : IProgramService
{
    private readonly ApplicationDbContext _context;

    public ProgramService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SkillProgramDto>> GetAllProgramsAsync(bool includeInactive = false)
    {
        var query = _context.SkillPrograms
            .Include(p => p.ProgramCourses)
                .ThenInclude(pc => pc.Course)
            .Include(p => p.Applications)
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        var programs = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return programs.Select(MapToProgramDto).ToList();
    }

    public async Task<SkillProgramDto?> GetProgramByIdAsync(int id)
    {
        var program = await _context.SkillPrograms
            .Include(p => p.ProgramCourses)
                .ThenInclude(pc => pc.Course)
            .Include(p => p.Applications)
            .FirstOrDefaultAsync(p => p.Id == id);

        return program != null ? MapToProgramDto(program) : null;
    }

    public async Task<SkillProgramDto> CreateProgramAsync(CreateProgramDto dto)
    {
        var program = new SkillProgram
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Industry = dto.Industry,
            DurationWeeks = dto.DurationWeeks,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxParticipants = dto.MaxParticipants
        };

        _context.SkillPrograms.Add(program);
        await _context.SaveChangesAsync();

        if (dto.CourseIds != null && dto.CourseIds.Any())
        {
            var orderIndex = 0;
            foreach (var courseId in dto.CourseIds)
            {
                _context.ProgramCourses.Add(new ProgramCourse
                {
                    ProgramId = program.Id,
                    CourseId = courseId,
                    OrderIndex = orderIndex++
                });
            }
            await _context.SaveChangesAsync();
        }

        return (await GetProgramByIdAsync(program.Id))!;
    }

    public async Task<SkillProgramDto?> UpdateProgramAsync(int id, UpdateProgramDto dto)
    {
        var program = await _context.SkillPrograms.FindAsync(id);
        if (program == null) return null;

        if (dto.Title != null) program.Title = dto.Title;
        if (dto.Description != null) program.Description = dto.Description;
        if (dto.ImageUrl != null) program.ImageUrl = dto.ImageUrl;
        if (dto.Industry != null) program.Industry = dto.Industry;
        if (dto.DurationWeeks.HasValue) program.DurationWeeks = dto.DurationWeeks.Value;
        if (dto.StartDate.HasValue) program.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) program.EndDate = dto.EndDate.Value;
        if (dto.MaxParticipants.HasValue) program.MaxParticipants = dto.MaxParticipants.Value;
        if (dto.IsActive.HasValue) program.IsActive = dto.IsActive.Value;

        program.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetProgramByIdAsync(id);
    }

    public async Task<bool> DeleteProgramAsync(int id)
    {
        var program = await _context.SkillPrograms.FindAsync(id);
        if (program == null) return false;

        _context.SkillPrograms.Remove(program);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddCourseToProgramAsync(int programId, int courseId, int orderIndex)
    {
        var exists = await _context.ProgramCourses
            .AnyAsync(pc => pc.ProgramId == programId && pc.CourseId == courseId);

        if (exists) return false;

        _context.ProgramCourses.Add(new ProgramCourse
        {
            ProgramId = programId,
            CourseId = courseId,
            OrderIndex = orderIndex
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveCourseFromProgramAsync(int programId, int courseId)
    {
        var programCourse = await _context.ProgramCourses
            .FirstOrDefaultAsync(pc => pc.ProgramId == programId && pc.CourseId == courseId);

        if (programCourse == null) return false;

        _context.ProgramCourses.Remove(programCourse);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProgramApplicationDto> ApplyToProgramAsync(string userId, CreateProgramApplicationDto dto)
    {
        var existing = await _context.ProgramApplications
            .AnyAsync(pa => pa.ProgramId == dto.ProgramId && pa.UserId == userId);

        if (existing)
            throw new InvalidOperationException("You have already applied to this program");

        var application = new ProgramApplication
        {
            ProgramId = dto.ProgramId,
            UserId = userId,
            CoverLetter = dto.CoverLetter
        };

        _context.ProgramApplications.Add(application);
        await _context.SaveChangesAsync();

        application = await _context.ProgramApplications
            .Include(pa => pa.Program)
            .Include(pa => pa.User)
            .FirstOrDefaultAsync(pa => pa.Id == application.Id);

        return MapToProgramApplicationDto(application!);
    }

    public async Task<List<ProgramApplicationDto>> GetUserProgramApplicationsAsync(string userId)
    {
        var applications = await _context.ProgramApplications
            .Include(pa => pa.Program)
            .Include(pa => pa.User)
            .Where(pa => pa.UserId == userId)
            .OrderByDescending(pa => pa.AppliedAt)
            .ToListAsync();

        return applications.Select(MapToProgramApplicationDto).ToList();
    }

    public async Task<List<ProgramApplicationDto>> GetProgramApplicationsAsync(int programId)
    {
        var applications = await _context.ProgramApplications
            .Include(pa => pa.Program)
            .Include(pa => pa.User)
            .Where(pa => pa.ProgramId == programId)
            .OrderByDescending(pa => pa.AppliedAt)
            .ToListAsync();

        return applications.Select(MapToProgramApplicationDto).ToList();
    }

    public async Task<ProgramApplicationDto?> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto)
    {
        var application = await _context.ProgramApplications
            .Include(pa => pa.Program)
            .Include(pa => pa.User)
            .FirstOrDefaultAsync(pa => pa.Id == applicationId);

        if (application == null) return null;

        if (Enum.TryParse<ApplicationStatus>(dto.Status, out var status))
        {
            application.Status = status;
            application.ReviewNotes = dto.ReviewNotes;
            application.ReviewedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return MapToProgramApplicationDto(application);
    }

    // Job methods
    public async Task<List<JobDto>> GetAllJobsAsync(bool includeInactive = false)
    {
        var query = _context.Jobs
            .Include(j => j.Applications)
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(j => j.IsActive && (j.ExpiresAt == null || j.ExpiresAt > DateTime.UtcNow));

        var jobs = await query.OrderByDescending(j => j.PostedAt).ToListAsync();
        return jobs.Select(MapToJobDto).ToList();
    }

    public async Task<JobDto?> GetJobByIdAsync(int id)
    {
        var job = await _context.Jobs
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        return job != null ? MapToJobDto(job) : null;
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto dto)
    {
        var job = new Job
        {
            Title = dto.Title,
            Company = dto.Company,
            Description = dto.Description,
            Location = dto.Location,
            SalaryRange = dto.SalaryRange,
            JobType = dto.JobType,
            RequiredSkills = dto.RequiredSkills,
            ExpiresAt = dto.ExpiresAt
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return MapToJobDto(job);
    }

    public async Task<JobDto?> UpdateJobAsync(int id, UpdateJobDto dto)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;

        if (dto.Title != null) job.Title = dto.Title;
        if (dto.Company != null) job.Company = dto.Company;
        if (dto.Description != null) job.Description = dto.Description;
        if (dto.Location != null) job.Location = dto.Location;
        if (dto.SalaryRange != null) job.SalaryRange = dto.SalaryRange;
        if (dto.JobType != null) job.JobType = dto.JobType;
        if (dto.RequiredSkills != null) job.RequiredSkills = dto.RequiredSkills;
        if (dto.IsActive.HasValue) job.IsActive = dto.IsActive.Value;
        if (dto.ExpiresAt.HasValue) job.ExpiresAt = dto.ExpiresAt.Value;

        await _context.SaveChangesAsync();
        return await GetJobByIdAsync(id);
    }

    public async Task<bool> DeleteJobAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<JobApplicationDto> ApplyToJobAsync(string userId, CreateJobApplicationDto dto)
    {
        var existing = await _context.JobApplications
            .AnyAsync(ja => ja.JobId == dto.JobId && ja.UserId == userId);

        if (existing)
            throw new InvalidOperationException("You have already applied to this job");

        var application = new JobApplication
        {
            JobId = dto.JobId,
            UserId = userId,
            CoverLetter = dto.CoverLetter,
            ResumeUrl = dto.ResumeUrl
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        application = await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.User)
            .FirstOrDefaultAsync(ja => ja.Id == application.Id);

        return MapToJobApplicationDto(application!);
    }

    public async Task<List<JobApplicationDto>> GetUserJobApplicationsAsync(string userId)
    {
        var applications = await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.User)
            .Where(ja => ja.UserId == userId)
            .OrderByDescending(ja => ja.AppliedAt)
            .ToListAsync();

        return applications.Select(MapToJobApplicationDto).ToList();
    }

    public async Task<List<JobApplicationDto>> GetJobApplicationsAsync(int jobId)
    {
        var applications = await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.User)
            .Where(ja => ja.JobId == jobId)
            .OrderByDescending(ja => ja.AppliedAt)
            .ToListAsync();

        return applications.Select(MapToJobApplicationDto).ToList();
    }

    public async Task<JobApplicationDto?> UpdateJobApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto)
    {
        var application = await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.User)
            .FirstOrDefaultAsync(ja => ja.Id == applicationId);

        if (application == null) return null;

        if (Enum.TryParse<ApplicationStatus>(dto.Status, out var status))
        {
            application.Status = status;
            application.ReviewNotes = dto.ReviewNotes;
            application.ReviewedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return MapToJobApplicationDto(application);
    }

    private static SkillProgramDto MapToProgramDto(SkillProgram program)
    {
        return new SkillProgramDto
        {
            Id = program.Id,
            Title = program.Title,
            Description = program.Description,
            ImageUrl = program.ImageUrl,
            Industry = program.Industry,
            DurationWeeks = program.DurationWeeks,
            StartDate = program.StartDate,
            EndDate = program.EndDate,
            MaxParticipants = program.MaxParticipants,
            CurrentParticipants = program.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0,
            IsActive = program.IsActive,
            CreatedAt = program.CreatedAt,
            Courses = program.ProgramCourses?.OrderBy(pc => pc.OrderIndex).Select(pc => new ProgramCourseDto
            {
                Id = pc.Id,
                CourseId = pc.CourseId,
                CourseTitle = pc.Course?.Title ?? string.Empty,
                OrderIndex = pc.OrderIndex
            }).ToList() ?? new List<ProgramCourseDto>()
        };
    }

    private static ProgramApplicationDto MapToProgramApplicationDto(ProgramApplication application)
    {
        return new ProgramApplicationDto
        {
            Id = application.Id,
            ProgramId = application.ProgramId,
            ProgramTitle = application.Program?.Title ?? string.Empty,
            UserId = application.UserId,
            UserName = $"{application.User?.FirstName} {application.User?.LastName}",
            UserEmail = application.User?.Email,
            CoverLetter = application.CoverLetter,
            Status = application.Status.ToString(),
            ReviewNotes = application.ReviewNotes,
            AppliedAt = application.AppliedAt,
            ReviewedAt = application.ReviewedAt
        };
    }

    private static JobDto MapToJobDto(Job job)
    {
        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Company = job.Company,
            Description = job.Description,
            Location = job.Location,
            SalaryRange = job.SalaryRange,
            JobType = job.JobType,
            RequiredSkills = job.RequiredSkills,
            IsActive = job.IsActive,
            PostedAt = job.PostedAt,
            ExpiresAt = job.ExpiresAt,
            ApplicationCount = job.Applications?.Count ?? 0
        };
    }

    private static JobApplicationDto MapToJobApplicationDto(JobApplication application)
    {
        return new JobApplicationDto
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = application.Job?.Title ?? string.Empty,
            Company = application.Job?.Company ?? string.Empty,
            UserId = application.UserId,
            UserName = $"{application.User?.FirstName} {application.User?.LastName}",
            UserEmail = application.User?.Email,
            CoverLetter = application.CoverLetter,
            ResumeUrl = application.ResumeUrl,
            Status = application.Status.ToString(),
            ReviewNotes = application.ReviewNotes,
            AppliedAt = application.AppliedAt,
            ReviewedAt = application.ReviewedAt
        };
    }
}
