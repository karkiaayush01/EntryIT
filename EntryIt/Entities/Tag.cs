namespace EntryIt.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

public class Tag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "pre-defined";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}