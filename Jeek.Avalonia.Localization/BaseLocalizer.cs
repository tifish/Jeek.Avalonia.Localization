using System.Collections.ObjectModel;
using System.Globalization;

namespace Jeek.Avalonia.Localization;

public abstract class BaseLocalizer : ILocalizer
{
    // Fallback language when the current language is not found
    public string FallbackLanguage { get; set; } = "en";

    protected readonly List<string> _languages = [];

    // List of available languages, e.g. ["en", "zh"]
    public List<string> Languages
    {
        get
        {
            if (!_hasLoaded)
                Reload();

            return _languages;
        }
    }

    // Display names of available languages in current language, e.g. ["English", "Chinese"]
    public ObservableCollection<string> DisplayLanguages { get; } = new FastObservableCollection<string>();

    protected string _language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

    // Current language, e.g. "en"
    public string Language
    {
        get => _language;
        set
        {
            if (_language == value)
                return;

            _language = value;

            OnLanguageChanged();

            FireLanguageChanged();
        }
    }

    private int _languageIndex = -1;

    // Index of current language in Languages
    public int LanguageIndex
    {
        get => _languageIndex;
        set
        {
            if (_languageIndex == value)
                return;

            if (value < 0 || value >= _languages.Count)
                return;

            _languageIndex = value;

            Language = _languages[_languageIndex];
        }
    }

    // Must be called after _language or _languages are changed
    protected void ValidateLanguage()
    {
        var languageIndex = _languages.IndexOf(_language);
        if (languageIndex != -1)
        {
            LanguageIndex = languageIndex;
            return;
        }

        languageIndex = _languages.IndexOf(FallbackLanguage);
        if (languageIndex == -1)
            throw new KeyNotFoundException(_language);

        LanguageIndex = languageIndex;
        _language = FallbackLanguage;
    }

    // Must be called after _languages are changed or translation is changed
    protected void UpdateDisplayLanguages()
    {
        var displayLanguages = Languages.Select(Get).ToList();
        if (!displayLanguages.SequenceEqual(DisplayLanguages))
            ((IFastObservableCollection)DisplayLanguages).Replace(displayLanguages);
    }

    // Implementations deal with loading language strings
    protected abstract void OnLanguageChanged();

    protected bool _hasLoaded;

    // Reload language strings
    public abstract void Reload();

    // Get language string by key
    public abstract string Get(string key);

    public event EventHandler? LanguageChanged;

    // Fire language changed
    public void FireLanguageChanged()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
