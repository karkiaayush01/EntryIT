namespace EntryIt.Services;

public class ThemeService
{
    private string CurrentTheme { get; set; } = "light";
    private string CurrentAccent { get; set; } = "default";

    public event Action? OnChange;

    public string GetCurrentTheme()
    {
        return this.CurrentTheme;
    }

    public void SetCurrentTheme(string theme)
    {
        this.CurrentTheme = theme;
        NotifyStateChanged();
    }

    public string GetCurrentAccent()
    {
        return this.CurrentAccent;
    }

    public void SetCurrentAccent(string accent)
    {
        this.CurrentAccent = accent;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
