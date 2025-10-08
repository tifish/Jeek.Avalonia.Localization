using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jeek.Avalonia.Localization.Example.Models;

namespace Jeek.Avalonia.Localization.Example;

public partial class MainViewModel : LocalizedViewModel
{
    [ObservableProperty] private int _languageIndex = Localizer.LanguageIndex;
    [ObservableProperty] private string _firstName = "First";
    [ObservableProperty] private string _lastName = "Last";
    
    [ObservableProperty] private User _user = new();

    public string WelcomeName { get; set; } = string.Empty;
    public string WelcomeUser { get; set; } = string.Empty;
    
    public MainViewModel()
    {
        Localize(() => WelcomeName, "WelcomeName", () => _firstName, () => LastName);
        Localize(() => WelcomeUser, "WelcomeUser", () => User.FirstName, () => User.LastName, () => User.Country.Name);
        Task.Run(() =>
        {
            Thread.Sleep(5000);
            FirstName = "John";
            LastName = "Doe";
            User.FirstName = "Jane";
            User.LastName = "Smith";
            Thread.Sleep(1000);
            User.Country.Name = "United States";
            Thread.Sleep(3000);
            User = new User
            {
                FirstName = "Another",
                LastName = "One",
                Country = new Country { Name = "Canada" }
            };
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
