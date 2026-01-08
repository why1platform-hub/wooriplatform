using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllCoursesAsync(bool includeUnpublished = false);
    Task<CourseDto?> GetCourseByIdAsync(int id, string? userId = null);
    Task<List<CourseDto>> GetCoursesByInstructorAsync(string instructorId);
    Task<CourseDto> CreateCourseAsync(string instructorId, CreateCourseDto dto);
    Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto dto);
    Task<bool> DeleteCourseAsync(int id);

    Task<CourseModuleDto> CreateModuleAsync(int courseId, CreateModuleDto dto);
    Task<CourseModuleDto?> UpdateModuleAsync(int moduleId, CreateModuleDto dto);
    Task<bool> DeleteModuleAsync(int moduleId);

    Task<LessonDto> CreateLessonAsync(int moduleId, CreateLessonDto dto);
    Task<LessonDto?> UpdateLessonAsync(int lessonId, CreateLessonDto dto);
    Task<bool> DeleteLessonAsync(int lessonId);

    Task<EnrollmentDto> EnrollUserAsync(string userId, int courseId);
    Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(string userId);
    Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId);
    Task<bool> UpdateProgressAsync(string userId, UpdateProgressDto dto);
    Task<bool> UnenrollUserAsync(string userId, int courseId);
}
