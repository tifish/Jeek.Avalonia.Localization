using System.Collections.ObjectModel;

namespace Jeek.Avalonia.Localization;

public static class Localizer
{
    private static ILocalizer _localizer = new TestLocalizer();

    public static void SetLocalizer(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    public static List<string> Languages => _localizer.Languages;

    public static ObservableCollection<string> DisplayLanguages => _localizer.DisplayLanguages;

    public static string Language
    {
        get => _localizer.Language;
        set => _localizer.Language = value;
    }

    public static int LanguageIndex
    {
        get => _localizer.LanguageIndex;
        set => _localizer.LanguageIndex = value;
    }

    public static string Get(string key)
    {
        return _localizer.Get(key);
    }

    public static event EventHandler? LanguageChanged
    {
        add => _localizer.LanguageChanged += value;
        remove => _localizer.LanguageChanged -= value;
    }
}
