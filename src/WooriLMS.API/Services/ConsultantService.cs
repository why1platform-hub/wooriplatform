using Microsoft.EntityFrameworkCore;
using WooriLMS.API.Data;
using WooriLMS.API.DTOs;
using WooriLMS.API.Models;

namespace WooriLMS.API.Services;

public class ConsultantService : IConsultantService
{
    private readonly ApplicationDbContext _context;

    public ConsultantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsultantDto>> GetAllConsultantsAsync()
    {
        var instructors = await _context.Users
            .Where(u => u.UserType == UserType.Instructor && u.IsActive)
            .ToListAsync();

        var consultants = new List<ConsultantDto>();

        foreach (var instructor in instructors)
        {
            var availableSlots = await _context.ConsultantTimeSlots
                .CountAsync(ts => ts.InstructorId == instructor.Id &&
                    ts.IsAvailable &&
                    ts.StartTime > DateTime.UtcNow);

            consultants.Add(new ConsultantDto
            {
                Id = instructor.Id,
                Name = $"{instructor.FirstName} {instructor.LastName}",
                ProfileImageUrl = instructor.ProfileImageUrl,
                Bio = instructor.Bio,
                Skills = instructor.Skills,
                AvailableSlotsCount = availableSlots
            });
        }

        return consultants;
    }

    public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(string? instructorId = null)
    {
        var query = _context.ConsultantTimeSlots
            .Include(ts => ts.Instructor)
            .Where(ts => ts.IsAvailable && ts.StartTime > DateTime.UtcNow);

        if (!string.IsNullOrEmpty(instructorId))
            query = query.Where(ts => ts.InstructorId == instructorId);

        var slots = await query.OrderBy(ts => ts.StartTime).ToListAsync();

        return slots.Select(MapToTimeSlotDto).ToList();
    }

    public async Task<List<TimeSlotDto>> GetInstructorTimeSlotsAsync(string instructorId)
    {
        var slots = await _context.ConsultantTimeSlots
            .Include(ts => ts.Instructor)
            .Include(ts => ts.Booking)
                .ThenInclude(b => b!.User)
            .Where(ts => ts.InstructorId == instructorId && ts.StartTime > DateTime.UtcNow.AddDays(-7))
            .OrderBy(ts => ts.StartTime)
            .ToListAsync();

        return slots.Select(MapToTimeSlotDto).ToList();
    }

    public async Task<TimeSlotDto> CreateTimeSlotAsync(string instructorId, CreateTimeSlotDto dto)
    {
        var slot = new ConsultantTimeSlot
        {
            InstructorId = instructorId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes,
            IsAvailable = true
        };

        _context.ConsultantTimeSlots.Add(slot);
        await _context.SaveChangesAsync();

        slot = await _context.ConsultantTimeSlots
            .Include(ts => ts.Instructor)
            .FirstOrDefaultAsync(ts => ts.Id == slot.Id);

        return MapToTimeSlotDto(slot!);
    }

    public async Task<List<TimeSlotDto>> CreateMultipleTimeSlotsAsync(string instructorId, CreateMultipleTimeSlotsDto dto)
    {
        var slots = new List<ConsultantTimeSlot>();
        var currentDate = dto.StartDate.Date;

        while (currentDate <= dto.EndDate.Date)
        {
            if (dto.DaysOfWeek.Count == 0 || dto.DaysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var slotStart = currentDate.Add(dto.SlotStartTime);
                var slotEnd = currentDate.Add(dto.SlotEndTime);

                while (slotStart.AddMinutes(dto.SlotDurationMinutes) <= slotEnd)
                {
                    slots.Add(new ConsultantTimeSlot
                    {
                        InstructorId = instructorId,
                        StartTime = slotStart,
                        EndTime = slotStart.AddMinutes(dto.SlotDurationMinutes),
                        IsAvailable = true
                    });

                    slotStart = slotStart.AddMinutes(dto.SlotDurationMinutes);
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        _context.ConsultantTimeSlots.AddRange(slots);
        await _context.SaveChangesAsync();

        var createdSlots = await _context.ConsultantTimeSlots
            .Include(ts => ts.Instructor)
            .Where(ts => slots.Select(s => s.Id).Contains(ts.Id))
            .ToListAsync();

        return createdSlots.Select(MapToTimeSlotDto).ToList();
    }

    public async Task<bool> DeleteTimeSlotAsync(int timeSlotId)
    {
        var slot = await _context.ConsultantTimeSlots
            .Include(ts => ts.Booking)
            .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

        if (slot == null) return false;

        if (slot.Booking != null)
            throw new InvalidOperationException("Cannot delete a time slot with an existing booking");

        _context.ConsultantTimeSlots.Remove(slot);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BookingDto> CreateBookingAsync(string userId, CreateBookingDto dto)
    {
        var slot = await _context.ConsultantTimeSlots
            .Include(ts => ts.Instructor)
            .FirstOrDefaultAsync(ts => ts.Id == dto.TimeSlotId);

        if (slot == null)
            throw new InvalidOperationException("Time slot not found");

        if (!slot.IsAvailable)
            throw new InvalidOperationException("Time slot is not available");

        var booking = new ConsultantBooking
        {
            TimeSlotId = dto.TimeSlotId,
            UserId = userId,
            Topic = dto.Topic,
            Description = dto.Description,
            Status = BookingStatus.Pending
        };

        slot.IsAvailable = false;

        _context.ConsultantBookings.Add(booking);
        await _context.SaveChangesAsync();

        booking = await _context.ConsultantBookings
            .Include(b => b.User)
            .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Instructor)
            .FirstOrDefaultAsync(b => b.Id == booking.Id);

        return MapToBookingDto(booking!);
    }

    public async Task<List<BookingDto>> GetUserBookingsAsync(string userId)
    {
        var bookings = await _context.ConsultantBookings
            .Include(b => b.User)
            .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Instructor)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.TimeSlot.StartTime)
            .ToListAsync();

        return bookings.Select(MapToBookingDto).ToList();
    }

    public async Task<List<BookingDto>> GetInstructorBookingsAsync(string instructorId)
    {
        var bookings = await _context.ConsultantBookings
            .Include(b => b.User)
            .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Instructor)
            .Where(b => b.TimeSlot.InstructorId == instructorId)
            .OrderByDescending(b => b.TimeSlot.StartTime)
            .ToListAsync();

        return bookings.Select(MapToBookingDto).ToList();
    }

    public async Task<BookingDto?> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusDto dto)
    {
        var booking = await _context.ConsultantBookings
            .Include(b => b.User)
            .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Instructor)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return null;

        if (Enum.TryParse<BookingStatus>(dto.Status, out var status))
        {
            booking.Status = status;

            if (status == BookingStatus.Approved)
            {
                booking.ApprovedAt = DateTime.UtcNow;
                booking.MeetingUrl = dto.MeetingUrl;
            }
            else if (status == BookingStatus.Rejected)
            {
                booking.CancellationReason = dto.CancellationReason;
                booking.TimeSlot.IsAvailable = true;
            }
        }

        await _context.SaveChangesAsync();
        return MapToBookingDto(booking);
    }

    public async Task<bool> CancelBookingAsync(int bookingId, string userId, string? reason = null)
    {
        var booking = await _context.ConsultantBookings
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return false;

        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = reason;
        booking.CancelledAt = DateTime.UtcNow;
        booking.TimeSlot.IsAvailable = true;

        await _context.SaveChangesAsync();
        return true;
    }

    private static TimeSlotDto MapToTimeSlotDto(ConsultantTimeSlot slot)
    {
        return new TimeSlotDto
        {
            Id = slot.Id,
            InstructorId = slot.InstructorId,
            InstructorName = $"{slot.Instructor?.FirstName} {slot.Instructor?.LastName}",
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            IsAvailable = slot.IsAvailable,
            Notes = slot.Notes,
            Booking = slot.Booking != null ? MapToBookingDto(slot.Booking) : null
        };
    }

    private static BookingDto MapToBookingDto(ConsultantBooking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            TimeSlotId = booking.TimeSlotId,
            UserId = booking.UserId,
            UserName = $"{booking.User?.FirstName} {booking.User?.LastName}",
            UserEmail = booking.User?.Email,
            InstructorId = booking.TimeSlot?.InstructorId ?? string.Empty,
            InstructorName = $"{booking.TimeSlot?.Instructor?.FirstName} {booking.TimeSlot?.Instructor?.LastName}",
            StartTime = booking.TimeSlot?.StartTime ?? DateTime.MinValue,
            EndTime = booking.TimeSlot?.EndTime ?? DateTime.MinValue,
            Topic = booking.Topic,
            Description = booking.Description,
            Status = booking.Status.ToString(),
            MeetingUrl = booking.MeetingUrl,
            CancellationReason = booking.CancellationReason,
            CreatedAt = booking.CreatedAt,
            ApprovedAt = booking.ApprovedAt
        };
    }
}
