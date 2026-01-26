namespace EntryIt.Models;

public class JournalLockStatus
{
    public Guid JournalId { get; set; }
    public bool IsLocked { get; set; }
}
