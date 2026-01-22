public class SaveResponse
{
    public Guid JournalId { get; set; } = Guid.Empty;
    public string Message { get; set; } = string.Empty;
    public int UpdatedStreak { get; set; } = 0;
}
