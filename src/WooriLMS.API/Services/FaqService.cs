using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Data;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class FaqService : IFaqService
{
    private readonly ApplicationDbContext _context;

    public FaqService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FaqDto>> GetAllFaqsAsync(bool includeUnpublished = false)
    {
        var query = _context.FAQs.AsQueryable();

        if (!includeUnpublished)
            query = query.Where(f => f.IsPublished);

        var faqs = await query
            .OrderBy(f => f.Category)
            .ThenBy(f => f.OrderIndex)
            .ToListAsync();

        return faqs.Select(MapToFaqDto).ToList();
    }

    public async Task<FaqDto?> GetFaqByIdAsync(int id)
    {
        var faq = await _context.FAQs.FindAsync(id);
        return faq != null ? MapToFaqDto(faq) : null;
    }

    public async Task<List<FaqDto>> GetFaqsByCategoryAsync(string category)
    {
        var faqs = await _context.FAQs
            .Where(f => f.IsPublished && f.Category == category)
            .OrderBy(f => f.OrderIndex)
            .ToListAsync();

        return faqs.Select(MapToFaqDto).ToList();
    }

    public async Task<FaqDto> CreateFaqAsync(CreateFaqDto dto)
    {
        var faq = new FAQ
        {
            Question = dto.Question,
            Answer = dto.Answer,
            Category = dto.Category,
            OrderIndex = dto.OrderIndex,
            IsPublished = dto.IsPublished
        };

        _context.FAQs.Add(faq);
        await _context.SaveChangesAsync();

        return MapToFaqDto(faq);
    }

    public async Task<FaqDto?> UpdateFaqAsync(int id, UpdateFaqDto dto)
    {
        var faq = await _context.FAQs.FindAsync(id);
        if (faq == null) return null;

        if (dto.Question != null) faq.Question = dto.Question;
        if (dto.Answer != null) faq.Answer = dto.Answer;
        if (dto.Category != null) faq.Category = dto.Category;
        if (dto.OrderIndex.HasValue) faq.OrderIndex = dto.OrderIndex.Value;
        if (dto.IsPublished.HasValue) faq.IsPublished = dto.IsPublished.Value;

        faq.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToFaqDto(faq);
    }

    public async Task<bool> DeleteFaqAsync(int id)
    {
        var faq = await _context.FAQs.FindAsync(id);
        if (faq == null) return false;

        _context.FAQs.Remove(faq);
        await _context.SaveChangesAsync();
        return true;
    }

    private static FaqDto MapToFaqDto(FAQ faq)
    {
        return new FaqDto
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer,
            Category = faq.Category,
            OrderIndex = faq.OrderIndex,
            IsPublished = faq.IsPublished,
            CreatedAt = faq.CreatedAt
        };
    }
}
