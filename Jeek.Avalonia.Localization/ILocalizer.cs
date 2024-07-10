namespace Jeek.Avalonia.Localization;

public interface ILocalizer
{
    void Reload();
    List<string> Languages { get; }
    string Language { get; set; }
    string DefaultLanguage { get; set; }
    string Get(string key);
    event EventHandler? LanguageChanged;
}
