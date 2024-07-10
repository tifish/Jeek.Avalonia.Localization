using CommunityToolkit.Mvvm.ComponentModel;

namespace Jeek.Avalonia.Localization.Example;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _language = Localizer.Language;

    partial void OnLanguageChanged(string value)
    {
        Localizer.Language = value;
    }
}
