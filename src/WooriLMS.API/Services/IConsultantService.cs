using WooriLMS.API.DTOs;

namespace WooriLMS.API.Services;

public interface IConsultantService
{
    Task<List<ConsultantDto>> GetAllConsultantsAsync();
    Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(string? instructorId = null);
    Task<List<TimeSlotDto>> GetInstructorTimeSlotsAsync(string instructorId);
    Task<TimeSlotDto> CreateTimeSlotAsync(string instructorId, CreateTimeSlotDto dto);
    Task<List<TimeSlotDto>> CreateMultipleTimeSlotsAsync(string instructorId, CreateMultipleTimeSlotsDto dto);
    Task<bool> DeleteTimeSlotAsync(int timeSlotId);

    Task<BookingDto> CreateBookingAsync(string userId, CreateBookingDto dto);
    Task<List<BookingDto>> GetUserBookingsAsync(string userId);
    Task<List<BookingDto>> GetInstructorBookingsAsync(string instructorId);
    Task<BookingDto?> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusDto dto);
    Task<bool> CancelBookingAsync(int bookingId, string userId, string? reason = null);
}
