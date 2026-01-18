namespace EntryIt.Entities;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password {  get; set; } = string.Empty;
}