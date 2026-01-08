namespace WooriLMS.API.Models;

public class ConsultantTimeSlot
{
    public int Id { get; set; }
    public string InstructorId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ApplicationUser Instructor { get; set; } = null!;
    public virtual ConsultantBooking? Booking { get; set; }
}

public class ConsultantBooking
{
    public int Id { get; set; }
    public int TimeSlotId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? MeetingUrl { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public virtual ConsultantTimeSlot TimeSlot { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

public enum BookingStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3,
    Completed = 4
}
