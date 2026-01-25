using EntryIt.Entities;

namespace EntryIt.Models;

public class JournalExtraInfo
{
    public string Content { get; set; } = string.Empty;
    public string ContentRaw { get; set; } = string.Empty;
    public int WordCount { get; set; } = 0;
    public MoodViewModel PrimaryMood { get; set; } = new MoodViewModel();
    public MoodViewModel? SecondaryMood1 { get; set; } = null;
    public MoodViewModel? SecondaryMood2 { get; set; } = null;
    public List<Tag> SelectedTags { get; set; } = [];
}
