namespace EntryIt.Entities;
using System.ComponentModel.DataAnnotations;

public class Journal_Tag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JournalId { get; set; } = Guid.Empty;
    public Guid TagId { get; set; } = Guid.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}