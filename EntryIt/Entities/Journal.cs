namespace EntryIt.Entities;
using System.ComponentModel.DataAnnotations;

public class Journal
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public string JournalContent { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
