using CommunityToolkit.Mvvm.ComponentModel;

namespace Jeek.Avalonia.Localization.Example.Models;

public partial class User: ObservableObject
{
    [ObservableProperty] private string _firstName = "First";
    [ObservableProperty] private string _lastName = "Last";
    [ObservableProperty] private Country _country = new();
}