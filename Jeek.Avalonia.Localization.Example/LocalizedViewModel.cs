using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Jeek.Avalonia.Localization.Example;

public class LocalizedViewModel:ObservableObject
{
    private readonly List<LocalizedProperty> _localizedProperties = [];

    protected LocalizedViewModel()
    {
        Localizer.LanguageChanged += (_,_) => UpdateAll();
        PropertyChanged += UpdateChanged;
    }

    private void UpdateChanged(object? sender, PropertyChangedEventArgs e)
    {
        foreach (LocalizedProperty prop in _localizedProperties
                     .Where(prop => prop.Args.Any(arg => arg.Name == e.PropertyName)))
        {
            Update(prop);
        }
    }

    private void UpdateAll()
    {
        foreach (LocalizedProperty prop in _localizedProperties)
        {
            Update(prop);
        }
    }

    private void Update(LocalizedProperty prop)
    {
        object?[] args = prop.Args.Select(arg => arg.GetValue(this)).ToArray();
        prop.Property.SetValue(this, string.Format(Localizer.Get(prop.Key), args));
        OnPropertyChanged(prop.Property.Name);
    }

    protected void Localize(string propertyName,string key, params object[] args)
    {
        PropertyInfo? property = GetType().GetProperty(propertyName);
        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found in '{GetType().Name}'");
        
        PropertyInfo[] argProperties = args.Select(arg =>
        {
            switch (arg)
            {
                case string argName:
                {
                    PropertyInfo? argProperty = GetType().GetProperty(argName);
                    return argProperty == null ? throw new ArgumentException($"Argument property '{argName}' not found in '{GetType().Name}'") : argProperty;
                }
                case PropertyInfo argProperty:
                    return argProperty;
                default:
                    throw new ArgumentException($"Argument must be a property name or PropertyInfo");
            }
        }).ToArray();

        LocalizedProperty prop = new()
        {
            Property = property,
            Key = key,
            Args = argProperties
        };

        _localizedProperties.Add(prop);

        Update(prop);
    }
}

public struct LocalizedProperty
{
    public PropertyInfo Property { get; init; }
    public string Key { get; init; }
    public PropertyInfo[] Args { get; init; }
}