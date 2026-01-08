using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Data;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly ApplicationDbContext _context;

    public AnnouncementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AnnouncementDto>> GetAllAnnouncementsAsync(bool includeUnpublished = false)
    {
        var query = _context.Announcements
            .Include(a => a.CreatedBy)
            .AsQueryable();

        if (!includeUnpublished)
        {
            var now = DateTime.UtcNow;
            query = query.Where(a => a.IsPublished &&
                (a.PublishDate == null || a.PublishDate <= now) &&
                (a.ExpiryDate == null || a.ExpiryDate > now));
        }

        var announcements = await query
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        return announcements.Select(MapToAnnouncementDto).ToList();
    }

    public async Task<AnnouncementDto?> GetAnnouncementByIdAsync(int id)
    {
        var announcement = await _context.Announcements
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);

        return announcement != null ? MapToAnnouncementDto(announcement) : null;
    }

    public async Task<AnnouncementDto> CreateAnnouncementAsync(string userId, CreateAnnouncementDto dto)
    {
        var announcement = new Announcement
        {
            Title = dto.Title,
            Content = dto.Content,
            Type = Enum.TryParse<AnnouncementType>(dto.Type, out var type) ? type : AnnouncementType.General,
            Priority = Enum.TryParse<AnnouncementPriority>(dto.Priority, out var priority) ? priority : AnnouncementPriority.Normal,
            IsPublished = dto.IsPublished,
            PublishDate = dto.PublishDate,
            ExpiryDate = dto.ExpiryDate,
            CreatedById = userId
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        announcement = await _context.Announcements
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == announcement.Id);

        return MapToAnnouncementDto(announcement!);
    }

    public async Task<AnnouncementDto?> UpdateAnnouncementAsync(int id, UpdateAnnouncementDto dto)
    {
        var announcement = await _context.Announcements
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (announcement == null) return null;

        if (dto.Title != null) announcement.Title = dto.Title;
        if (dto.Content != null) announcement.Content = dto.Content;
        if (dto.Type != null && Enum.TryParse<AnnouncementType>(dto.Type, out var type))
            announcement.Type = type;
        if (dto.Priority != null && Enum.TryParse<AnnouncementPriority>(dto.Priority, out var priority))
            announcement.Priority = priority;
        if (dto.IsPublished.HasValue) announcement.IsPublished = dto.IsPublished.Value;
        if (dto.PublishDate.HasValue) announcement.PublishDate = dto.PublishDate.Value;
        if (dto.ExpiryDate.HasValue) announcement.ExpiryDate = dto.ExpiryDate.Value;

        announcement.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToAnnouncementDto(announcement);
    }

    public async Task<bool> DeleteAnnouncementAsync(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return false;

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
        return true;
    }

    private static AnnouncementDto MapToAnnouncementDto(Announcement announcement)
    {
        return new AnnouncementDto
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            Type = announcement.Type.ToString(),
            Priority = announcement.Priority.ToString(),
            IsPublished = announcement.IsPublished,
            PublishDate = announcement.PublishDate,
            ExpiryDate = announcement.ExpiryDate,
            CreatedById = announcement.CreatedById,
            CreatedByName = $"{announcement.CreatedBy?.FirstName} {announcement.CreatedBy?.LastName}",
            CreatedAt = announcement.CreatedAt
        };
    }
}
