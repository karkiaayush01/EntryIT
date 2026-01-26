namespace EntryIt.Models;

public class JournalUnlockResponse
{
    public Guid JournalId { get; set; }
    public bool HasUnlocked { get; set; }
    public string Message {  get; set; } = string.Empty;
}
