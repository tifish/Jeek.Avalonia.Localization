using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Jeek.Avalonia.Localization;

class FastObservableCollection<T> : ObservableCollection<T>, IFastObservableCollection
{
    private bool _suppressChangedEvent;

    public void Replace(IEnumerable<T> other)
    {
        _suppressChangedEvent = true;

        Clear();
        AddRange(other);
    }

    public void Replace(IEnumerable other)
    {
        _suppressChangedEvent = true;

        Clear();

        foreach (T item in other)
            Add(item);

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    }

    public void AddRange(IEnumerable other)
    {
        _suppressChangedEvent = true;

        foreach (var item in other)
            if (item is T tItem)
                Add(tItem);

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    }

    public void RemoveRange(IEnumerable other)
    {
        _suppressChangedEvent = true;

        foreach (var item in other)
            if (item is T tItem)
                Remove(tItem);

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    }

    public void SortAndReplace(IEnumerable<T> other, IComparer<T> comparer)
    {
        List<T> values = new(other);
        values.Sort(comparer);
        Replace(values);
    }

    public void Sort(IComparer<T> comparer)
    {
        List<T> values = new(this);
        values.Sort(comparer);
        Replace(values);
    }

    public void Synchronize(NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            AddRange(args.NewItems);
        else if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
            RemoveRange(args.OldItems);
        else if (args.Action == NotifyCollectionChangedAction.Reset)
            Clear();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_suppressChangedEvent)
            return;

        base.OnPropertyChanged(e);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_suppressChangedEvent)
            return;

        base.OnCollectionChanged(e);
    }
}

public interface IFastObservableCollection
{
    public void Replace(IEnumerable other);
}
