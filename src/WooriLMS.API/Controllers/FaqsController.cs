using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaqsController : ControllerBase
{
    private readonly IFaqService _faqService;

    public FaqsController(IFaqService faqService)
    {
        _faqService = faqService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FaqDto>>> GetAllFaqs([FromQuery] bool includeUnpublished = false)
    {
        var isAdmin = User.IsInRole("Admin");
        var faqs = await _faqService.GetAllFaqsAsync(isAdmin && includeUnpublished);
        return Ok(faqs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FaqDto>> GetFaq(int id)
    {
        var faq = await _faqService.GetFaqByIdAsync(id);
        if (faq == null)
            return NotFound();

        return Ok(faq);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<FaqDto>>> GetFaqsByCategory(string category)
    {
        var faqs = await _faqService.GetFaqsByCategoryAsync(category);
        return Ok(faqs);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<FaqDto>> CreateFaq([FromBody] CreateFaqDto dto)
    {
        var faq = await _faqService.CreateFaqAsync(dto);
        return CreatedAtAction(nameof(GetFaq), new { id = faq.Id }, faq);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<FaqDto>> UpdateFaq(int id, [FromBody] UpdateFaqDto dto)
    {
        var faq = await _faqService.UpdateFaqAsync(id, dto);
        if (faq == null)
            return NotFound();

        return Ok(faq);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFaq(int id)
    {
        var result = await _faqService.DeleteFaqAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
