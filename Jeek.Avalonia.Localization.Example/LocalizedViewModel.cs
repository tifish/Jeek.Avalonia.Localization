using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Jeek.Avalonia.Localization.Example;

public class LocalizedViewModel:ObservableObject
{
    private readonly List<LocalizedProperty> _localizedProperties = [];
    private readonly Dictionary<object, List<LocalizedProperty>> _nestedObjectSubscriptions = [];

    protected LocalizedViewModel()
    {
        Localizer.LanguageChanged += (_,_) => UpdateAll();
        PropertyChanged += UpdateChanged;
    }

    private void UpdateChanged(object? sender, PropertyChangedEventArgs e)
    {
        foreach (LocalizedProperty prop in _localizedProperties
                     .Where(prop => prop.Args.Any(arg => arg.RootPropertyName == e.PropertyName)))
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

    private void SubscribeToNestedObject(object nestedObject, LocalizedProperty localizedProperty)
    {
        if (nestedObject is not INotifyPropertyChanged notifyPropertyChanged) return;
        if (!_nestedObjectSubscriptions.TryGetValue(nestedObject, out List<LocalizedProperty>? value))
        {
            value = [];
            _nestedObjectSubscriptions[nestedObject] = value;
            notifyPropertyChanged.PropertyChanged += OnNestedPropertyChanged;
        }
            
        if (!value.Contains(localizedProperty))
        {
            value.Add(localizedProperty);
        }
    }

    private void OnNestedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender == null || !_nestedObjectSubscriptions.TryGetValue(sender, out List<LocalizedProperty>? properties)) return;
        foreach (LocalizedProperty prop in properties.Where(p => p.Args.Any(arg => 
                     IsPropertyPathAffected(arg.PropertyPath, sender, e.PropertyName ?? string.Empty))))
        {
            Update(prop);
                
            // Re-subscribe to nested objects in case the chain changed
            foreach (PropertyArgument arg in prop.Args.Where(a => a.PropertyChain.Length > 1))
            {
                SubscribeToNestedObjectsInChain(this, arg.PropertyChain, prop);
            }
        }
    }

    private bool IsPropertyPathAffected(string propertyPath, object changedObject, string changedPropertyName)
    {
        // If the property name matches the end of the path, it's affected
        if (propertyPath.EndsWith("." + changedPropertyName))
            return true;
            
        // If an intermediate object in the chain changed, we need to update too
        // This handles cases where the entire nested object is replaced
        string[] pathParts = propertyPath.Split('.');
        object? current = this;
        
        for (int i = 0; i < pathParts.Length - 1; i++)
        {
            if (current == null) break;
            
            PropertyInfo? propInfo = current.GetType().GetProperty(pathParts[i]);
            if (propInfo == null) break;
            
            current = propInfo.GetValue(current);
            
            // If we found the changed object and the property name matches
            if (current == changedObject && pathParts[i + 1] == changedPropertyName)
                return true;
        }
        
        return false;
    }

    protected void Localize<T>(Expression<Func<T>> propertyExpression, string key, params Expression<Func<object?>>[] argExpressions)
    {
        string propertyName = GetPropertyName(propertyExpression);
        object[] args = argExpressions.Select(GetPropertyPath).ToArray<object>();
        Localize(propertyName, key, args);
    }

    private void Localize(string propertyName, string key, params object[] args)
    {
        PropertyInfo? property = GetType().GetProperty(propertyName);
        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found in '{GetType().Name}'");
        
        PropertyArgument[] argProperties = args.Select(arg =>
        {
            return arg switch
            {
                string argName => CreatePropertyArgument(argName),
                PropertyInfo argProperty => new PropertyArgument 
                { 
                    PropertyPath = argProperty.Name,
                    RootPropertyName = argProperty.Name,
                    PropertyChain = [argProperty]
                },
                _ => throw new ArgumentException("Argument must be a property name or PropertyInfo")
            };
        }).ToArray();

        LocalizedProperty prop = new()
        {
            Property = property,
            Key = key,
            Args = argProperties
        };

        _localizedProperties.Add(prop);

        // Subscribe to all nested objects in the property chain
        foreach (PropertyArgument arg in argProperties.Where(a => a.PropertyChain.Length > 1))
        {
            SubscribeToNestedObjectsInChain(this, arg.PropertyChain, prop);
        }

        Update(prop);
    }

    private void SubscribeToNestedObjectsInChain(object root, PropertyInfo[] propertyChain, LocalizedProperty localizedProperty)
    {
        object? current = root;
        
        // Subscribe to each object in the chain (except the last property which is the value)
        for (int i = 0; i < propertyChain.Length - 1; i++)
        {
            if (current == null) break;
            
            current = propertyChain[i].GetValue(current);
            if (current != null)
            {
                SubscribeToNestedObject(current, localizedProperty);
            }
        }
    }

    private PropertyArgument CreatePropertyArgument(string propertyPath)
    {
        string[] pathParts = propertyPath.Split('.');
        PropertyInfo[] propertyChain = new PropertyInfo[pathParts.Length];
        Type currentType = GetType();

        for (int i = 0; i < pathParts.Length; i++)
        {
            PropertyInfo? propInfo = currentType.GetProperty(pathParts[i]);
            if (propInfo == null)
                throw new ArgumentException($"Property '{pathParts[i]}' not found in '{currentType.Name}'");
            
            propertyChain[i] = propInfo;
            currentType = propInfo.PropertyType;
        }

        return new PropertyArgument
        {
            PropertyPath = propertyPath,
            RootPropertyName = pathParts[0],
            PropertyChain = propertyChain
        };
    }

    private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }
        throw new ArgumentException("Expression must be a property", nameof(propertyExpression));
    }

    private static string GetPropertyPath(Expression<Func<object?>> expression)
    {
        Expression? memberExpression = expression.Body;
        
        // Handle conversion expressions (boxing to object)
        if (memberExpression is UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression)
        {
            memberExpression = unaryExpression.Operand;
        }

        List<string> pathParts = [];
        
        while (memberExpression is MemberExpression member)
        {
            pathParts.Insert(0, member.Member.Name);
            memberExpression = member.Expression;
        }

        return pathParts.Count == 0 
            ? throw new ArgumentException("Expression must be a property or property path", nameof(expression)) 
            : string.Join(".", pathParts);
    }
}

public class LocalizedProperty
{
    public PropertyInfo Property { get; init; } = null!;
    public string Key { get; init; } = string.Empty;
    public PropertyArgument[] Args { get; init; } = [];
}

public class PropertyArgument
{
    public string PropertyPath { get; init; } = string.Empty;
    public string RootPropertyName { get; init; } = string.Empty;
    public PropertyInfo[] PropertyChain { get; init; } = [];

    public object? GetValue(object root)
    {
        object? current = root;
        foreach (PropertyInfo prop in PropertyChain)
        {
            if (current == null) return null;
            current = prop.GetValue(current);
        }
        return current;
    }
}