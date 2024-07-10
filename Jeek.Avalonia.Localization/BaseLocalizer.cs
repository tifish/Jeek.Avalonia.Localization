using System.Globalization;

namespace Jeek.Avalonia.Localization;

public abstract class BaseLocalizer : ILocalizer
{
    public string DefaultLanguage { get; set; } = "en-US";

    protected readonly List<string> _languages = [];

    public List<string> Languages
    {
        get
        {
            if (!_hasLoaded)
                Reload();

            return _languages;
        }
    }

    protected string _language = CultureInfo.CurrentCulture.Name;

    public string Language
    {
        get => _language;
        set => SetLanguage(value);
    }

    protected abstract void SetLanguage(string language);

    protected bool _hasLoaded;

    public abstract void Reload();

    public abstract string Get(string key);

    public event EventHandler? LanguageChanged;

    public void RefreshUI()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
