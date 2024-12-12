using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Jeek.Avalonia.Localization.Example;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Set the localizer type, default to TestLocalizer
        // Localizer.SetLocalizer(new JsonLocalizer());
        // Localizer.SetLocalizer(new TabLocalizer());
        Localizer.SetLocalizer(new ResXLocalizer());

        // Set language, default to en
        // Localizer.Language = "en";

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}
