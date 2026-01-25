namespace EntryIt.Models;

public class JournalSearchFilters
{
    public string SearchKey = string.Empty;
    public DateTime? FromDate { get; set; } = null;
    public DateTime ToDate { get; set; } = DateTime.Now.Date;
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}
