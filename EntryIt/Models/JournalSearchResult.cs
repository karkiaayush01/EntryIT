namespace EntryIt.Models;

public class JournalSearchResult
{
    public List<JournalSearchResponse> Results { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}
