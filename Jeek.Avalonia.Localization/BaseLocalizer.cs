namespace Jeek.Avalonia.Localization;

public class BaseLocalizer
{
    public event EventHandler? LanguageChanged;

    public void RefreshUI()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
