using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IAnnouncementService
{
    Task<List<AnnouncementDto>> GetAllAnnouncementsAsync(bool includeUnpublished = false);
    Task<AnnouncementDto?> GetAnnouncementByIdAsync(int id);
    Task<AnnouncementDto> CreateAnnouncementAsync(string userId, CreateAnnouncementDto dto);
    Task<AnnouncementDto?> UpdateAnnouncementAsync(int id, UpdateAnnouncementDto dto);
    Task<bool> DeleteAnnouncementAsync(int id);
}
