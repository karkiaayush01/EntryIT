namespace EntryIt.Models;
public class JournalSearchResponse
{
    public Guid JournalId { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime SaveDate { get; set; } = DateTime.Now.Date;
    public bool IsLocked { get; set; } = false;

    // Only return journal data if not locked
    public JournalExtraInfo? JournalInfo { get; set; } = null;
}
