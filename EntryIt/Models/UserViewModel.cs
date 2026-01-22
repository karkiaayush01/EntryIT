public class UserViewModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email {  get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
}
