using CommunityToolkit.Mvvm.ComponentModel;

namespace Jeek.Avalonia.Localization.Example.Models;

public partial class Country:ObservableObject
{
    [ObservableProperty] private string _name = "USA";
}