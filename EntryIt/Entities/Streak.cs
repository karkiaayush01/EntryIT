namespace EntryIt.Entities;
using System.ComponentModel.DataAnnotations;

public class Streak
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.Empty;

    // User Date is the local date storage used to store activity according to user's local device time and helps to track missed days.
    public DateTime ActivityDate { get; set; } = DateTime.Now.Date;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}