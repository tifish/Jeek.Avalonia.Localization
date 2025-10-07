using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Jeek.Avalonia.Localization.Example;

public partial class MainViewModel : LocalizedViewModel
{
    [ObservableProperty] private int _languageIndex = Localizer.LanguageIndex;
    [ObservableProperty] private string _firstName = "First";
    [ObservableProperty] private string _lastName = "Last";

    public string WelcomeName { get; set; } = string.Empty;
    
    public MainViewModel()
    {
        Localize(nameof(WelcomeName), "WelcomeName", nameof(FirstName), nameof(LastName));
        Task.Run(() =>
        {
            Task.Delay(5000).Wait();
            FirstName = "John";
            LastName = "Doe";
        });
    }

    partial void OnLanguageIndexChanged(int value)
    {
        Localizer.LanguageIndex = value;
    }

    [ObservableProperty]
    private string _welcomeMessage = "";

    [RelayCommand]
    private void Welcome()
    {
        WelcomeMessage = Localizer.Get("Welcome");
    }
}
