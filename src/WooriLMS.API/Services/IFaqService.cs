using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IFaqService
{
    Task<List<FaqDto>> GetAllFaqsAsync(bool includeUnpublished = false);
    Task<FaqDto?> GetFaqByIdAsync(int id);
    Task<List<FaqDto>> GetFaqsByCategoryAsync(string category);
    Task<FaqDto> CreateFaqAsync(CreateFaqDto dto);
    Task<FaqDto?> UpdateFaqAsync(int id, UpdateFaqDto dto);
    Task<bool> DeleteFaqAsync(int id);
}
