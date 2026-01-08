using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseDto>>> GetAllCourses([FromQuery] bool includeUnpublished = false)
    {
        var isAdmin = User.IsInRole("Admin");
        var courses = await _courseService.GetAllCoursesAsync(isAdmin && includeUnpublished);
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _courseService.GetCourseByIdAsync(id, userId);
        if (course == null)
            return NotFound();

        return Ok(course);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpGet("instructor")]
    public async Task<ActionResult<List<CourseDto>>> GetInstructorCourses()
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var courses = await _courseService.GetCoursesByInstructorAsync(instructorId);
        return Ok(courses);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var course = await _courseService.CreateCourseAsync(instructorId, dto);
        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
    {
        var course = await _courseService.UpdateCourseAsync(id, dto);
        if (course == null)
            return NotFound();

        return Ok(course);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCourse(int id)
    {
        var result = await _courseService.DeleteCourseAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // Modules
    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPost("{courseId}/modules")]
    public async Task<ActionResult<CourseModuleDto>> CreateModule(int courseId, [FromBody] CreateModuleDto dto)
    {
        var module = await _courseService.CreateModuleAsync(courseId, dto);
        return Ok(module);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPut("modules/{moduleId}")]
    public async Task<ActionResult<CourseModuleDto>> UpdateModule(int moduleId, [FromBody] CreateModuleDto dto)
    {
        var module = await _courseService.UpdateModuleAsync(moduleId, dto);
        if (module == null)
            return NotFound();

        return Ok(module);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpDelete("modules/{moduleId}")]
    public async Task<ActionResult> DeleteModule(int moduleId)
    {
        var result = await _courseService.DeleteModuleAsync(moduleId);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // Lessons
    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPost("modules/{moduleId}/lessons")]
    public async Task<ActionResult<LessonDto>> CreateLesson(int moduleId, [FromBody] CreateLessonDto dto)
    {
        var lesson = await _courseService.CreateLessonAsync(moduleId, dto);
        return Ok(lesson);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPut("lessons/{lessonId}")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(int lessonId, [FromBody] CreateLessonDto dto)
    {
        var lesson = await _courseService.UpdateLessonAsync(lessonId, dto);
        if (lesson == null)
            return NotFound();

        return Ok(lesson);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpDelete("lessons/{lessonId}")]
    public async Task<ActionResult> DeleteLesson(int lessonId)
    {
        var result = await _courseService.DeleteLessonAsync(lessonId);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // Enrollments
    [Authorize]
    [HttpPost("{courseId}/enroll")]
    public async Task<ActionResult<EnrollmentDto>> EnrollInCourse(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var enrollment = await _courseService.EnrollUserAsync(userId, courseId);
            return Ok(enrollment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("enrollments")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetMyEnrollments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var enrollments = await _courseService.GetUserEnrollmentsAsync(userId);
        return Ok(enrollments);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpGet("{courseId}/enrollments")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetCourseEnrollments(int courseId)
    {
        var enrollments = await _courseService.GetCourseEnrollmentsAsync(courseId);
        return Ok(enrollments);
    }

    [Authorize]
    [HttpPost("progress")]
    public async Task<ActionResult> UpdateProgress([FromBody] UpdateProgressDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _courseService.UpdateProgressAsync(userId, dto);
        if (!result)
            return BadRequest();

        return Ok(new { message = "Progress updated" });
    }

    [Authorize]
    [HttpDelete("{courseId}/unenroll")]
    public async Task<ActionResult> UnenrollFromCourse(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _courseService.UnenrollUserAsync(userId, courseId);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
