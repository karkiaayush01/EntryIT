namespace EntryIt.Entities;
using System.ComponentModel.DataAnnotations;

public class Journal
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; } = Guid.Empty;
    public int WordCount { get; set; } = 0;
    public string PrimaryMood { get; set; } = string.Empty;
    public string SecondaryMood1 { get; set; } = string.Empty;
    public string SecondaryMood2 { get; set; } = string.Empty;
    public bool IsLocked { get; set; } = false;
    public string Password {  get; set; } = string.Empty;
    public DateTime SaveDate { get; set; } = DateTime.Now.Date; //Local Device date for easy retrieval
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}
