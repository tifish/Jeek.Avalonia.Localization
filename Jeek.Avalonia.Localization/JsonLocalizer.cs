using System.Text.Json;

namespace Jeek.Avalonia.Localization;

public class JsonLocalizer(string languageJsonDirectory = "") : BaseLocalizer
{
    private readonly string _languageJsonDirectory =
        languageJsonDirectory != ""
            ? languageJsonDirectory
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");

    private Dictionary<string, string>? _languageStrings;

    public override void Reload()
    {
        _languageStrings = null;
        _languages.Clear();

        if (!Directory.Exists(_languageJsonDirectory))
            throw new FileNotFoundException(_languageJsonDirectory);

        foreach (var file in Directory.GetFiles(_languageJsonDirectory, "*.json"))
        {
            var language = Path.GetFileNameWithoutExtension(file);
            _languages.Add(language);
        }

        ValidateLanguage();

        var languageFile = Path.Combine(_languageJsonDirectory, _language + ".json");
        if (!File.Exists(languageFile))
            throw new FileNotFoundException($"No language file ${languageFile}");

        var json = File.ReadAllText(languageFile);
        _languageStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

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

        if (_languageStrings == null)
            throw new Exception("No language strings loaded.");

        if (_languageStrings.TryGetValue(key, out var langStr))
            return langStr.Replace("\\n", "\n");

        return $"{Language}:{key}";
    }
}
