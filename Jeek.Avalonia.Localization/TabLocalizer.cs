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

        UpdateLanguageIndex();

        _hasLoaded = true;
    }

    private int _currentLanguageIndex = -1;

    private void UpdateLanguageIndex()
    {
        _currentLanguageIndex = _languages.IndexOf(_language);
        if (_currentLanguageIndex != -1)
            return;

        _currentLanguageIndex = _languages.IndexOf(DefaultLanguage);
        if (_currentLanguageIndex == -1)
            throw new KeyNotFoundException(_language);

        _language = DefaultLanguage;
    }

    protected override void SetLanguage(string language)
    {
        _language = language;

        if (_hasLoaded)
            Reload();
        else
            UpdateLanguageIndex();

        RefreshUI();
    }

    public override string Get(string key)
    {
        if (!_hasLoaded)
            Reload();

        if (_languageStrings.TryGetValue(key, out var langStrings))
            return langStrings[_currentLanguageIndex].Replace("\\n", "\n");

        return $"{Language}:{key}";
    }
}
