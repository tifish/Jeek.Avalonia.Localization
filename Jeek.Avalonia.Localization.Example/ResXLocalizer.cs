using System.Globalization;
using Jeek.Avalonia.Localization.Example.Assets;

namespace Jeek.Avalonia.Localization.Example;

public class ResXLocalizer : BaseLocalizer
{
    public override void Reload()
    {
        if (_languages.Count == 0)
        {
            _languages.Add("eu-US");
            _languages.Add("zh-CN");
            _hasLoaded = true;
        }

        Resources.Culture = new CultureInfo(_language);
    }

    protected override void SetLanguage(string language)
    {
        _language = language;

        Reload();

        RefreshUI();
    }

    public override string Get(string key)
    {
        if (!_hasLoaded)
            Reload();

        var langString = Resources.ResourceManager.GetString(key, Resources.Culture);
        return langString != null
            ? langString.Replace("\\n", "\n")
            : $"{Language}:{key}";
    }
}
