using EntryIt.Entities;

namespace EntryIt.Models;

public class FilterDates
{
    public DateTime? FromDate { get; set; } = null;
    public DateTime ToDate { get; set; }
    public List<Guid> MoodIds { get; set; } = [];
    public List<Guid> TagIds { get; set; } = [];
}
