namespace Jeek.Avalonia.Localization;

public class TestLocalizer: BaseLocalizer
{
    protected override void OnLanguageChanged()
    {
        // Do nothing
    }

    public override void Reload()
    {
        // Do nothing
    }

    public override string Get(string key)
    {
        return $"Key: {key}";
    }
}
