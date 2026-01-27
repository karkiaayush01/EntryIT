namespace EntryIt.Models;

public class JournalSearchFilters
{
    public string SearchKey = string.Empty;
    public DateTime? FromDate { get; set; } = null;
    public DateTime ToDate { get; set; } = DateTime.Now.Date;
    public List<Guid> Moods { get; set; } = [];
    public List<Guid> Tags { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}
