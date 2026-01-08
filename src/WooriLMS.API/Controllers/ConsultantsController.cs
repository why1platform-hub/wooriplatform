using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WooriLMS.API.DTOs;
using WooriLMS.API.Services;

namespace WooriLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultantsController : ControllerBase
{
    private readonly IConsultantService _consultantService;

    public ConsultantsController(IConsultantService consultantService)
    {
        _consultantService = consultantService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ConsultantDto>>> GetAllConsultants()
    {
        var consultants = await _consultantService.GetAllConsultantsAsync();
        return Ok(consultants);
    }

    [HttpGet("timeslots")]
    public async Task<ActionResult<List<TimeSlotDto>>> GetAvailableTimeSlots([FromQuery] string? instructorId = null)
    {
        var slots = await _consultantService.GetAvailableTimeSlotsAsync(instructorId);
        return Ok(slots);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpGet("my-timeslots")]
    public async Task<ActionResult<List<TimeSlotDto>>> GetMyTimeSlots()
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var slots = await _consultantService.GetInstructorTimeSlotsAsync(instructorId);
        return Ok(slots);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPost("timeslots")]
    public async Task<ActionResult<TimeSlotDto>> CreateTimeSlot([FromBody] CreateTimeSlotDto dto)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var slot = await _consultantService.CreateTimeSlotAsync(instructorId, dto);
        return Ok(slot);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPost("timeslots/bulk")]
    public async Task<ActionResult<List<TimeSlotDto>>> CreateMultipleTimeSlots([FromBody] CreateMultipleTimeSlotsDto dto)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var slots = await _consultantService.CreateMultipleTimeSlotsAsync(instructorId, dto);
        return Ok(slots);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpDelete("timeslots/{id}")]
    public async Task<ActionResult> DeleteTimeSlot(int id)
    {
        try
        {
            var result = await _consultantService.DeleteTimeSlotAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Bookings
    [Authorize]
    [HttpPost("bookings")]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var booking = await _consultantService.CreateBookingAsync(userId, dto);
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("bookings")]
    public async Task<ActionResult<List<BookingDto>>> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var bookings = await _consultantService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpGet("bookings/instructor")]
    public async Task<ActionResult<List<BookingDto>>> GetInstructorBookings()
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var bookings = await _consultantService.GetInstructorBookingsAsync(instructorId);
        return Ok(bookings);
    }

    [Authorize(Policy = "InstructorOrAdmin")]
    [HttpPut("bookings/{id}/status")]
    public async Task<ActionResult<BookingDto>> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        var booking = await _consultantService.UpdateBookingStatusAsync(id, dto);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [Authorize]
    [HttpPost("bookings/{id}/cancel")]
    public async Task<ActionResult> CancelBooking(int id, [FromBody] string? reason = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _consultantService.CancelBookingAsync(id, userId, reason);
        if (!result)
            return NotFound();

        return Ok(new { message = "Booking cancelled" });
    }
}
