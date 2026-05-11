using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Jeek.Avalonia.Localization;

public class LocalizeExtension : MarkupExtension
{
    private static readonly IMultiValueConverter LocalizeKeyConverter =
        new LocalizeKeyMultiValueConverter();

    private readonly string? _key;
    private readonly BindingBase? _keyBinding;

    public LocalizeExtension(string key)
    {
        _key = key;
    }

    public LocalizeExtension(BindingBase keyBinding)
    {
        _keyBinding = keyBinding;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (_keyBinding != null)
        {
            return new MultiBinding
            {
                Bindings = { _keyBinding, new LanguageChangedObservable().ToBinding() },
                Mode = BindingMode.OneWay,
                Converter = LocalizeKeyConverter,
            };
        }

        return new LocalizedStringObservable(_key ?? "").ToBinding();
    }

    private sealed class LocalizedStringObservable(string key) : IObservable<string>
    {
        public IDisposable Subscribe(IObserver<string> observer)
        {
            void Publish() => observer.OnNext(Localizer.Get(key));

            void OnLanguageChanged(object? sender, EventArgs e) => Publish();

            Publish();
            Localizer.LanguageChanged += OnLanguageChanged;

            return new EventSubscription(() => Localizer.LanguageChanged -= OnLanguageChanged);
        }
    }

    private sealed class LanguageChangedObservable : IObservable<int>
    {
        private int _version;

        public IDisposable Subscribe(IObserver<int> observer)
        {
            void Publish() => observer.OnNext(_version++);

            void OnLanguageChanged(object? sender, EventArgs e) => Publish();

            Publish();
            Localizer.LanguageChanged += OnLanguageChanged;

            return new EventSubscription(() => Localizer.LanguageChanged -= OnLanguageChanged);
        }
    }

    private sealed class LocalizeKeyMultiValueConverter : IMultiValueConverter
    {
        public object? Convert(
            IList<object?> values,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            if (values.Count == 0)
                return BindingOperations.DoNothing;

            var key = values[0];
            if (key is BindingNotification { HasValue: false })
                return BindingOperations.DoNothing;

            if (key is BindingNotification bindingNotification)
                key = bindingNotification.Value;

            if (
                ReferenceEquals(key, AvaloniaProperty.UnsetValue)
                || ReferenceEquals(key, BindingOperations.DoNothing)
            )
            {
                return BindingOperations.DoNothing;
            }

            if (key is null)
                return "";

            var keyString = key as string ?? key.ToString();
            if (string.IsNullOrEmpty(keyString))
                return "";

            return Localizer.Get(keyString);
        }
    }

    private sealed class EventSubscription(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;

        public void Dispose()
        {
            _unsubscribe?.Invoke();
            _unsubscribe = null;
        }
    }
}
