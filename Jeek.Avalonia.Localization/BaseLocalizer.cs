namespace Jeek.Avalonia.Localization;

public class BaseLocalizer
{
    public string DefaultLanguage { get; set; } = "en-US";

    public event EventHandler? LanguageChanged;

    public void RefreshUI()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
