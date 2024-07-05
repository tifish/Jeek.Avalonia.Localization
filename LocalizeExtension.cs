using Avalonia;
using Avalonia.Markup.Xaml;

namespace Jeek.Avalonia.Localization;

public class LocalizeExtension(string Key) : MarkupExtension, IObservable<string>, IDisposable
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this.ToBinding();
    }

    private IObserver<string>? _observer;

    public IDisposable Subscribe(IObserver<string> observer)
    {
        _observer = observer;
        _observer.OnNext(Localizer.Get(Key));
        Localizer.LanguageChanged += OnLanguageChanged;

        return this;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        _observer?.OnNext(Localizer.Get(Key));
    }

    public void Dispose()
    {
        Localizer.LanguageChanged -= OnLanguageChanged;
        _observer = null;

        GC.SuppressFinalize(this);
    }
}
