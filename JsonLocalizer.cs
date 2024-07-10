using System.Text.Json;

namespace Jeek.Avalonia.Localization;

public class JsonLocalizer(string languageJsonDirectory = "") : BaseLocalizer, ILocalizer
{
    private Dictionary<string, string>? _languageStrings;

    private readonly string _languageJsonDirectory =
        languageJsonDirectory != ""
            ? languageJsonDirectory
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");

    private bool _hasLoaded;

    private readonly List<string> _languages = [];

    public List<string> Languages
    {
        get
        {
            if (!_hasLoaded)
                Reload();

            return _languages;
        }
    }

    public void Reload()
    {
        _languageStrings = null;

        if (!Directory.Exists(_languageJsonDirectory))
            throw new FileNotFoundException(_languageJsonDirectory);

        foreach (var file in Directory.GetFiles(_languageJsonDirectory, "*.json"))
        {
            var language = Path.GetFileNameWithoutExtension(file);
            _languages.Add(language);
        }

        if (_language != "")
        {
            var languageFile = Path.Combine(_languageJsonDirectory, _language + ".json");
            if (!_languages.Contains(_language))
                throw new FileNotFoundException($"No language file ${languageFile}");

            var json = File.ReadAllText(languageFile);
            _languageStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        _hasLoaded = true;
    }

    private string _language = "";

    public string Language
    {
        get => _language;
        set
        {
            _language = value;

            Reload();

            RefreshUI();
        }
    }

    public string Get(string key)
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
