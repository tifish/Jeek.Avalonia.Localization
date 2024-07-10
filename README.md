# Jeek.Avalonia.Localization
Avalonia string localization support.

- Switch language at runtime.
- Customizable language file format and location.
- Simple interface, simple implementation.

## Usage

- Use localized string in .axaml: 

```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        
        <!-- Add namespace -->
        xmlns:l="using:Jeek.Avalonia.Localization"

        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="Jeek.Avalonia.Localization.Example.MainWindow"
        Title="Jeek.Avalonia.Localization.Example">
    <StackPanel>
        
        <!-- Get the string with key Welcome -->
        <Label Content="{l:Localize Welcome}" />
        
    </StackPanel>
</Window>
```

- Json language file `Languages\en-US.json`:

```json
{
    "Welcome": "Welcome to Avalonia!"
}
```

- Switch language:

```c#
Localizer.Language = "en-US";
```

- Get localized string in code:

```c#
Localizer.Get("Welcome");
```

## Origin

- I found [Localizing using ResX | Avalonia Docs (avaloniaui.net)](https://docs.avaloniaui.net/docs/guides/implementation-guides/localizing) in Avalonia website. It's static and can't switch language at runtime.
- Then I found this article: [Avalonia UI Framework localization | Sakya's Homepage](https://www.sakya.it/wordpress/avalonia-ui-framework-localization/), and learn much from it. Finally I created my own version, more simple and flexible.
