using System.Text;

namespace Jeek.Avalonia.Localization;

public class TabLocalizer(string tabFilePath = "") : BaseLocalizer, ILocalizer
{
    private readonly string _tabFilePath =
        tabFilePath != ""
            ? tabFilePath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.tab");

    private readonly Dictionary<string, string[]> _languageStrings = [];

    public override void Reload()
    {
        _languageStrings.Clear();

        if (!File.Exists(_tabFilePath))
            throw new FileNotFoundException(_tabFilePath);

        using var reader = new StreamReader(_tabFilePath, Encoding.UTF8);
        var headerLine = reader.ReadLine();
        if (headerLine == null)
            throw new Exception("File is empty.");

        var headers = headerLine.Split('\t');
        _languages.Clear();
        _languages.AddRange(headers.Skip(1).ToList());

        var line = reader.ReadLine();
        while (line != null)
        {
            var values = line.Split('\t');
            _languageStrings[values[0]] = values.Skip(1).ToArray();

            line = reader.ReadLine();
        }

        ValidateLanguage();

        _hasLoaded = true;

        UpdateDisplayLanguages();
    }

    protected override void OnLanguageChanged()
    {
        if (!_hasLoaded)
        {
            Reload();
        }
        else
        {
            ValidateLanguage();
            UpdateDisplayLanguages();
        }
    }

    public override string Get(string key)
    {
        if (!_hasLoaded)
            Reload();

        if (_languageStrings.TryGetValue(key, out var langStrings))
            return langStrings[LanguageIndex].Replace("\\n", "\n");

        return $"{Language}:{key}";
    }
}
