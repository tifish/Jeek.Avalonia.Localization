using System.Text;

namespace Jeek.Avalonia.Localization;

public class TabLocalizer(string tabFilePath = "") : BaseLocalizer, ILocalizer
{
    private readonly string _tabFilePath =
        tabFilePath != ""
            ? tabFilePath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.tab");

    private readonly Dictionary<string, string[]> _languageStrings = [];

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

        _currentLanguageIndex = _languages.IndexOf(_language);
        if (_currentLanguageIndex == -1)
        {
            _currentLanguageIndex = _languages.IndexOf(DefaultLanguage);
            if (_currentLanguageIndex == -1)
                throw new KeyNotFoundException(_language);

            _language = DefaultLanguage;
        }

        _hasLoaded = true;
    }

    private int _currentLanguageIndex = -1;

    private string _language = "";

    public string Language
    {
        get => _language;
        set
        {
            if (_hasLoaded)
                Reload();

            _currentLanguageIndex = Languages.IndexOf(value);
            if (_currentLanguageIndex == -1)
                throw new KeyNotFoundException();

            _language = value;

            RefreshUI();
        }
    }

    public string Get(string key)
    {
        if (!_hasLoaded)
            Reload();

        if (_languageStrings.TryGetValue(key, out var langStrings))
            return langStrings[_currentLanguageIndex].Replace("\\n", "\n");

        return $"{Language}:{key}";
    }
}
