using System.Collections.ObjectModel;

namespace Jeek.Avalonia.Localization;

public interface ILocalizer
{
    void Reload();
    List<string> Languages { get; }
    ObservableCollection<string> DisplayLanguages { get; }
    string Language { get; set; }
    int LanguageIndex { get; set; }
    string FallbackLanguage { get; set; }
    string Get(string key);
    event EventHandler? LanguageChanged;
}
