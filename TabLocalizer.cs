using System.Text;

namespace Jeek.Avalonia.Localization;

public class TabLocalizer : ILocalizer
{
    public TabLocalizer(string tabFilePathPath = "")
    {
        if (tabFilePathPath != "" && File.Exists(tabFilePathPath))
            _tabFilePath = tabFilePathPath;
    }

    private readonly Dictionary<string, string[]> _languageStrings = [];

    private readonly string _tabFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.tab");

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
        Languages.Clear();
        Languages.AddRange(headers.Skip(1).ToList());

        var line = reader.ReadLine();
        while (line != null)
        {
            var values = line.Split('\t');
            _languageStrings[values[0]] = values.Skip(1).ToArray();

            line = reader.ReadLine();
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

            Invalidate();
        }
    }

    public string Get(string key)
    {
        if (!_hasLoaded)
            Reload();

        if (_languageStrings.TryGetValue(key, out var res))
            return res[_currentLanguageIndex].Replace("\\n", "\n");

        return $"{Language}:{key}";
    }

    public event EventHandler? LanguageChanged;

    public void Invalidate()
    {
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }
}
