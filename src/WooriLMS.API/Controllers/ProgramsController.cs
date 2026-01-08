using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramService _programService;

    public ProgramsController(IProgramService programService)
    {
        _programService = programService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SkillProgramDto>>> GetAllPrograms([FromQuery] bool includeInactive = false)
    {
        var isAdmin = User.IsInRole("Admin");
        var programs = await _programService.GetAllProgramsAsync(isAdmin && includeInactive);
        return Ok(programs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SkillProgramDto>> GetProgram(int id)
    {
        var program = await _programService.GetProgramByIdAsync(id);
        if (program == null)
            return NotFound();

        return Ok(program);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<SkillProgramDto>> CreateProgram([FromBody] CreateProgramDto dto)
    {
        var program = await _programService.CreateProgramAsync(dto);
        return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<SkillProgramDto>> UpdateProgram(int id, [FromBody] UpdateProgramDto dto)
    {
        var program = await _programService.UpdateProgramAsync(id, dto);
        if (program == null)
            return NotFound();

        return Ok(program);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProgram(int id)
    {
        var result = await _programService.DeleteProgramAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("{programId}/courses/{courseId}")]
    public async Task<ActionResult> AddCourseToProgram(int programId, int courseId, [FromQuery] int orderIndex = 0)
    {
        var result = await _programService.AddCourseToProgramAsync(programId, courseId, orderIndex);
        if (!result)
            return BadRequest(new { message = "Course already in program" });

        return Ok(new { message = "Course added to program" });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{programId}/courses/{courseId}")]
    public async Task<ActionResult> RemoveCourseFromProgram(int programId, int courseId)
    {
        var result = await _programService.RemoveCourseFromProgramAsync(programId, courseId);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // Applications
    [Authorize]
    [HttpPost("{programId}/apply")]
    public async Task<ActionResult<ProgramApplicationDto>> ApplyToProgram(int programId, [FromBody] CreateProgramApplicationDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        dto.ProgramId = programId;

        try
        {
            var application = await _programService.ApplyToProgramAsync(userId, dto);
            return Ok(application);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("my-applications")]
    public async Task<ActionResult<List<ProgramApplicationDto>>> GetMyApplications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var applications = await _programService.GetUserProgramApplicationsAsync(userId);
        return Ok(applications);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{programId}/applications")]
    public async Task<ActionResult<List<ProgramApplicationDto>>> GetProgramApplications(int programId)
    {
        var applications = await _programService.GetProgramApplicationsAsync(programId);
        return Ok(applications);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("applications/{applicationId}/status")]
    public async Task<ActionResult<ProgramApplicationDto>> UpdateApplicationStatus(int applicationId, [FromBody] UpdateApplicationStatusDto dto)
    {
        var application = await _programService.UpdateApplicationStatusAsync(applicationId, dto);
        if (application == null)
            return NotFound();

        return Ok(application);
    }
}
