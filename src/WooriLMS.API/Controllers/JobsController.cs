using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IProgramService _programService;

    public JobsController(IProgramService programService)
    {
        _programService = programService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobDto>>> GetAllJobs([FromQuery] bool includeInactive = false)
    {
        var isAdmin = User.IsInRole("Admin");
        var jobs = await _programService.GetAllJobsAsync(isAdmin && includeInactive);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobDto>> GetJob(int id)
    {
        var job = await _programService.GetJobByIdAsync(id);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<JobDto>> CreateJob([FromBody] CreateJobDto dto)
    {
        var job = await _programService.CreateJobAsync(dto);
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<JobDto>> UpdateJob(int id, [FromBody] UpdateJobDto dto)
    {
        var job = await _programService.UpdateJobAsync(id, dto);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteJob(int id)
    {
        var result = await _programService.DeleteJobAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // Applications
    [Authorize]
    [HttpPost("{jobId}/apply")]
    public async Task<ActionResult<JobApplicationDto>> ApplyToJob(int jobId, [FromBody] CreateJobApplicationDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        dto.JobId = jobId;

        try
        {
            var application = await _programService.ApplyToJobAsync(userId, dto);
            return Ok(application);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("my-applications")]
    public async Task<ActionResult<List<JobApplicationDto>>> GetMyApplications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var applications = await _programService.GetUserJobApplicationsAsync(userId);
        return Ok(applications);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{jobId}/applications")]
    public async Task<ActionResult<List<JobApplicationDto>>> GetJobApplications(int jobId)
    {
        var applications = await _programService.GetJobApplicationsAsync(jobId);
        return Ok(applications);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("applications/{applicationId}/status")]
    public async Task<ActionResult<JobApplicationDto>> UpdateApplicationStatus(int applicationId, [FromBody] UpdateApplicationStatusDto dto)
    {
        var application = await _programService.UpdateJobApplicationStatusAsync(applicationId, dto);
        if (application == null)
            return NotFound();

        return Ok(application);
    }
}
