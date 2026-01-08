using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Data;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseDto>> GetAllCoursesAsync(bool includeUnpublished = false)
    {
        var query = _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Include(c => c.Enrollments)
            .AsQueryable();

        if (!includeUnpublished)
            query = query.Where(c => c.IsPublished);

        var courses = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return courses.Select(MapToCourseDto).ToList();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id, string? userId = null)
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Modules.OrderBy(m => m.OrderIndex))
                .ThenInclude(m => m.Lessons.OrderBy(l => l.OrderIndex))
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        var dto = MapToCourseDto(course);

        if (userId != null)
        {
            var progresses = await _context.LessonProgresses
                .Where(lp => lp.UserId == userId && course.Modules
                    .SelectMany(m => m.Lessons)
                    .Select(l => l.Id)
                    .Contains(lp.LessonId))
                .ToListAsync();

            foreach (var module in dto.Modules)
            {
                foreach (var lesson in module.Lessons)
                {
                    var progress = progresses.FirstOrDefault(p => p.LessonId == lesson.Id);
                    if (progress != null)
                    {
                        lesson.IsCompleted = progress.IsCompleted;
                        lesson.WatchedSeconds = progress.WatchedSeconds;
                    }
                }
            }
        }

        return dto;
    }

    public async Task<List<CourseDto>> GetCoursesByInstructorAsync(string instructorId)
    {
        var courses = await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Include(c => c.Enrollments)
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return courses.Select(MapToCourseDto).ToList();
    }

    public async Task<CourseDto> CreateCourseAsync(string instructorId, CreateCourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            ThumbnailUrl = dto.ThumbnailUrl,
            Category = dto.Category,
            Level = dto.Level,
            InstructorId = instructorId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        course = await _context.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == course.Id);

        return MapToCourseDto(course!);
    }

    public async Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto dto)
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        if (dto.Title != null) course.Title = dto.Title;
        if (dto.Description != null) course.Description = dto.Description;
        if (dto.ThumbnailUrl != null) course.ThumbnailUrl = dto.ThumbnailUrl;
        if (dto.Category != null) course.Category = dto.Category;
        if (dto.Level != null) course.Level = dto.Level;
        if (dto.IsPublished.HasValue) course.IsPublished = dto.IsPublished.Value;

        course.UpdatedAt = DateTime.UtcNow;
        course.DurationMinutes = course.Modules.SelectMany(m => m.Lessons).Sum(l => l.DurationMinutes);

        await _context.SaveChangesAsync();
        return MapToCourseDto(course);
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CourseModuleDto> CreateModuleAsync(int courseId, CreateModuleDto dto)
    {
        var module = new CourseModule
        {
            CourseId = courseId,
            Title = dto.Title,
            Description = dto.Description,
            OrderIndex = dto.OrderIndex
        };

        _context.CourseModules.Add(module);
        await _context.SaveChangesAsync();

        return new CourseModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            OrderIndex = module.OrderIndex
        };
    }

    public async Task<CourseModuleDto?> UpdateModuleAsync(int moduleId, CreateModuleDto dto)
    {
        var module = await _context.CourseModules.FindAsync(moduleId);
        if (module == null) return null;

        module.Title = dto.Title;
        module.Description = dto.Description;
        module.OrderIndex = dto.OrderIndex;

        await _context.SaveChangesAsync();

        return new CourseModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            OrderIndex = module.OrderIndex
        };
    }

    public async Task<bool> DeleteModuleAsync(int moduleId)
    {
        var module = await _context.CourseModules.FindAsync(moduleId);
        if (module == null) return false;

        _context.CourseModules.Remove(module);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LessonDto> CreateLessonAsync(int moduleId, CreateLessonDto dto)
    {
        var lesson = new Lesson
        {
            ModuleId = moduleId,
            Title = dto.Title,
            Description = dto.Description,
            VideoUrl = dto.VideoUrl,
            DurationMinutes = dto.DurationMinutes,
            OrderIndex = dto.OrderIndex
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        // Update course duration
        var module = await _context.CourseModules.FindAsync(moduleId);
        if (module != null)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.Id == module.CourseId);

            if (course != null)
            {
                course.DurationMinutes = course.Modules.SelectMany(m => m.Lessons).Sum(l => l.DurationMinutes);
                await _context.SaveChangesAsync();
            }
        }

        return new LessonDto
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            DurationMinutes = lesson.DurationMinutes,
            OrderIndex = lesson.OrderIndex
        };
    }

    public async Task<LessonDto?> UpdateLessonAsync(int lessonId, CreateLessonDto dto)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return null;

        lesson.Title = dto.Title;
        lesson.Description = dto.Description;
        lesson.VideoUrl = dto.VideoUrl;
        lesson.DurationMinutes = dto.DurationMinutes;
        lesson.OrderIndex = dto.OrderIndex;

        await _context.SaveChangesAsync();

        return new LessonDto
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            DurationMinutes = lesson.DurationMinutes,
            OrderIndex = lesson.OrderIndex
        };
    }

    public async Task<bool> DeleteLessonAsync(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return false;

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<EnrollmentDto> EnrollUserAsync(string userId, int courseId)
    {
        var existing = await _context.CourseEnrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (existing != null)
        {
            throw new InvalidOperationException("User is already enrolled in this course");
        }

        var enrollment = new CourseEnrollment
        {
            UserId = userId,
            CourseId = courseId
        };

        _context.CourseEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        enrollment = await _context.CourseEnrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrollment.Id);

        return MapToEnrollmentDto(enrollment!);
    }

    public async Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(string userId)
    {
        var enrollments = await _context.CourseEnrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        return enrollments.Select(MapToEnrollmentDto).ToList();
    }

    public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId)
    {
        var enrollments = await _context.CourseEnrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        return enrollments.Select(MapToEnrollmentDto).ToList();
    }

    public async Task<bool> UpdateProgressAsync(string userId, UpdateProgressDto dto)
    {
        var progress = await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == dto.LessonId);

        if (progress == null)
        {
            progress = new LessonProgress
            {
                UserId = userId,
                LessonId = dto.LessonId,
                WatchedSeconds = dto.WatchedSeconds,
                IsCompleted = dto.IsCompleted,
                CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null
            };
            _context.LessonProgresses.Add(progress);
        }
        else
        {
            progress.WatchedSeconds = dto.WatchedSeconds;
            progress.IsCompleted = dto.IsCompleted;
            progress.LastWatchedAt = DateTime.UtcNow;
            if (dto.IsCompleted && progress.CompletedAt == null)
                progress.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Update enrollment progress
        var lesson = await _context.Lessons
            .Include(l => l.Module)
            .FirstOrDefaultAsync(l => l.Id == dto.LessonId);

        if (lesson != null)
        {
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == lesson.Module.CourseId);

            if (enrollment != null)
            {
                var totalLessons = await _context.Lessons
                    .CountAsync(l => l.Module.CourseId == lesson.Module.CourseId);

                var completedLessons = await _context.LessonProgresses
                    .CountAsync(lp => lp.UserId == userId && lp.IsCompleted &&
                        _context.Lessons.Any(l => l.Id == lp.LessonId && l.Module.CourseId == lesson.Module.CourseId));

                enrollment.ProgressPercentage = totalLessons > 0 ? (completedLessons * 100 / totalLessons) : 0;

                if (enrollment.ProgressPercentage >= 100)
                {
                    enrollment.Status = EnrollmentStatus.Completed;
                    enrollment.CompletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }

        return true;
    }

    public async Task<bool> UnenrollUserAsync(string userId, int courseId)
    {
        var enrollment = await _context.CourseEnrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null) return false;

        _context.CourseEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CourseDto MapToCourseDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            ThumbnailUrl = course.ThumbnailUrl,
            Category = course.Category,
            Level = course.Level,
            DurationMinutes = course.DurationMinutes,
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt,
            InstructorId = course.InstructorId,
            InstructorName = $"{course.Instructor?.FirstName} {course.Instructor?.LastName}",
            EnrollmentCount = course.Enrollments?.Count ?? 0,
            ModuleCount = course.Modules?.Count ?? 0,
            LessonCount = course.Modules?.SelectMany(m => m.Lessons).Count() ?? 0,
            Modules = course.Modules?.OrderBy(m => m.OrderIndex).Select(m => new CourseModuleDto
            {
                Id = m.Id,
                CourseId = m.CourseId,
                Title = m.Title,
                Description = m.Description,
                OrderIndex = m.OrderIndex,
                Lessons = m.Lessons?.OrderBy(l => l.OrderIndex).Select(l => new LessonDto
                {
                    Id = l.Id,
                    ModuleId = l.ModuleId,
                    Title = l.Title,
                    Description = l.Description,
                    VideoUrl = l.VideoUrl,
                    DurationMinutes = l.DurationMinutes,
                    OrderIndex = l.OrderIndex
                }).ToList() ?? new List<LessonDto>()
            }).ToList() ?? new List<CourseModuleDto>()
        };
    }

    private static EnrollmentDto MapToEnrollmentDto(CourseEnrollment enrollment)
    {
        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            UserName = $"{enrollment.User?.FirstName} {enrollment.User?.LastName}",
            CourseId = enrollment.CourseId,
            CourseTitle = enrollment.Course?.Title ?? string.Empty,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt,
            ProgressPercentage = enrollment.ProgressPercentage,
            Status = enrollment.Status.ToString()
        };
    }
}
