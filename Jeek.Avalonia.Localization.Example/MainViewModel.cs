using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Jeek.Avalonia.Localization.Example;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _language = Localizer.Language;

    partial void OnLanguageChanged(string value)
    {
        Localizer.Language = value;
    }

    [ObservableProperty]
    private string _welcomeMessage = "";

    [RelayCommand]
    private void Welcome()
    {
        WelcomeMessage = Localizer.Get("Welcome");
    }
}
