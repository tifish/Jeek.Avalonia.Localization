using System.Globalization;
using Jeek.Avalonia.Localization.Example.Assets;

namespace Jeek.Avalonia.Localization.Example;

public class ResXLocalizer : BaseLocalizer
{
    public override void Reload()
    {
        if (_languages.Count == 0)
        {
            _languages.Add("en");
            _languages.Add("zh");
        }

        ValidateLanguage();

        Resources.Culture = new CultureInfo(_language);

        _hasLoaded = true;

        UpdateDisplayLanguages();
    }

    protected override void OnLanguageChanged()
    {
        Reload();
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
