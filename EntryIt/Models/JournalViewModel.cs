namespace EntryIt.Models;

using EntryIt.Entities;

public class JournalViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int WordCount { get; set; } = 0;
    public Guid PrimaryMood { get; set; } = Guid.Empty;
    public Guid SecondaryMood1 { get; set; } = Guid.Empty;
    public Guid SecondaryMood2 { get; set; } = Guid.Empty;
    public bool IsLocked { get; set; } = false;
    public DateTime SaveDate { get; set; } = DateTime.Now.Date; //Local Device date for easy retrieval
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public List<Tag> SelectedTags { get; set; } = [];
}