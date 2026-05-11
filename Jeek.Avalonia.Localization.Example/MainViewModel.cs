using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Jeek.Avalonia.Localization.Example;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private int _languageIndex = Localizer.LanguageIndex;

    partial void OnLanguageIndexChanged(int value)
    {
        Localizer.LanguageIndex = value;
    }

    [ObservableProperty]
    private string _welcomeMessage = "";

    [ObservableProperty]
    private string _titleKey = "Welcome";

    [RelayCommand]
    private void Welcome()
    {
        WelcomeMessage = Localizer.Get("Welcome");
    }

    [RelayCommand]
    private void ToggleTitleKey()
    {
        TitleKey = TitleKey == "Welcome" ? "ClickMe" : "Welcome";
    }
}
