using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace Jeek.Avalonia.Localization.Example;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        OnLanguageChanged(null, EventArgs.Empty);
        Localizer.LanguageChanged += OnLanguageChanged;

        InitializeComponent();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        FontFamily = new FontFamily(Localizer.Get("DefaultFontName"));
    }
}
