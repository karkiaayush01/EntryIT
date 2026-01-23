
using EntryIt.Entities;
namespace EntryIt.Models;
public class JournalDataFromModal
{
    public string Title { get; set; } = string.Empty;
    public Mood? PrimaryMood { get; set; } = null;
    public List<Mood?> SecondaryMoods { get; set; } = [];
    public List<Tag> Tags { get; set; } = new();
    public bool IsLocked { get; set; }
    public string? Password { get; set; }
    public bool UseSamePassword { get; set; }
}