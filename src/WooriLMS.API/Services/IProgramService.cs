using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IProgramService
{
    // Programs
    Task<List<SkillProgramDto>> GetAllProgramsAsync(bool includeInactive = false);
    Task<SkillProgramDto?> GetProgramByIdAsync(int id);
    Task<SkillProgramDto> CreateProgramAsync(CreateProgramDto dto);
    Task<SkillProgramDto?> UpdateProgramAsync(int id, UpdateProgramDto dto);
    Task<bool> DeleteProgramAsync(int id);
    Task<bool> AddCourseToProgramAsync(int programId, int courseId, int orderIndex);
    Task<bool> RemoveCourseFromProgramAsync(int programId, int courseId);

    // Program Applications
    Task<ProgramApplicationDto> ApplyToProgramAsync(string userId, CreateProgramApplicationDto dto);
    Task<List<ProgramApplicationDto>> GetUserProgramApplicationsAsync(string userId);
    Task<List<ProgramApplicationDto>> GetProgramApplicationsAsync(int programId);
    Task<ProgramApplicationDto?> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto);

    // Jobs
    Task<List<JobDto>> GetAllJobsAsync(bool includeInactive = false);
    Task<JobDto?> GetJobByIdAsync(int id);
    Task<JobDto> CreateJobAsync(CreateJobDto dto);
    Task<JobDto?> UpdateJobAsync(int id, UpdateJobDto dto);
    Task<bool> DeleteJobAsync(int id);

    // Job Applications
    Task<JobApplicationDto> ApplyToJobAsync(string userId, CreateJobApplicationDto dto);
    Task<List<JobApplicationDto>> GetUserJobApplicationsAsync(string userId);
    Task<List<JobApplicationDto>> GetJobApplicationsAsync(int jobId);
    Task<JobApplicationDto?> UpdateJobApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto);
}
