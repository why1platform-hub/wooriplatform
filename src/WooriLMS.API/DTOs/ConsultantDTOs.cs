using System.ComponentModel.DataAnnotations;

namespace WooriLMS.API.DTOs;

public class ConsultantDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public int AvailableSlotsCount { get; set; }
}

public class TimeSlotDto
{
    public int Id { get; set; }
    public string InstructorId { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
    public BookingDto? Booking { get; set; }
}

public class CreateTimeSlotDto
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public string? Notes { get; set; }
}

public class CreateMultipleTimeSlotsDto
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public TimeSpan SlotStartTime { get; set; }

    [Required]
    public TimeSpan SlotEndTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 60;
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();
}

public class BookingDto
{
    public int Id { get; set; }
    public int TimeSlotId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string InstructorId { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MeetingUrl { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class CreateBookingDto
{
    [Required]
    public int TimeSlotId { get; set; }

    [Required]
    public string Topic { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class UpdateBookingStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty; // Approved, Rejected

    public string? MeetingUrl { get; set; }
    public string? CancellationReason { get; set; }
}
