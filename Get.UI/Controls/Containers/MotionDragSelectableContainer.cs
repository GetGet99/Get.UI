#nullable enable
namespace Get.UI.Controls.Containers;
[DependencyProperty(typeof(object), "PrimarySelectedItem", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
[DependencyProperty(typeof(int), "PrimarySelectedIndex", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(bool), "PreferAlwaysSelectItem", GenerateLocalOnPropertyChangedMethod = true)]
//[DependencyProperty(typeof(bool), "AllowMultipleSelection", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public partial class MotionDragSelectableContainer : MotionDragContainer, ISelectableContainer
{
    public MotionDragSelectableContainer()
    {
        DefaultStyleKey = typeof(MotionDragContainer);
    }

    protected override void OnItemsChanged(object e)
    {
        if (PrimarySelectedItem is { } item)
        {
            if (ContainerFromItem(item) is null)
            {
                selectedItemCache = item;
                PrimarySelectedItem = null;
            }
        }
        else
        {
            if (selectedItemCache is not null &&
                ContainerFromItem(selectedItemCache) is not null)
            {
                PrimarySelectedItem = selectedItemCache;
                selectedItemCache = null;
            }
        }
        var oldIdx = PrimarySelectedIndex;
        if (SafeContainerFromIndex(oldIdx)?.FindDescendantOrSelf<MotionDragSelectableItem>() is { } curIdxItem)
        {
            if (curIdxItem.IsPrimarySelected)
            {
                // we don't need to update selection
                return;
            }
        }
        for (int i = 0; i < ItemsCount; i++)
        {
            if (SafeContainerFromIndex(i)?.FindDescendantOrSelf<MotionDragSelectableItem>() is { } theItem)
                if (theItem.IsPrimarySelected)
                {
                    // we found the selected item
                    PrimarySelectedIndex = i;
                    while (SafeContainerFromIndex(++i)?.FindDescendantOrSelf<MotionDragSelectableItem>() is { } theItem2)
                    {
                        if (theItem2.IsPrimarySelected)
                        {
                            // wait there are multiple selected items?
                            Debugger.Break();
                            theItem2.IsPrimarySelected = false;
                            return;
                        }
                    }
                    return;
                }
        }
        // nothing is selected anymore...
        PrimarySelectedIndex = -1;
    }
    object? selectedItemCache;
    partial void OnPrimarySelectedIndexChanged(int oldValue, int newValue)
    {
        if (newValue is >= 0)
            PrimarySelectedItem = IsLoaded ?
                ItemFromContainer(ContainerFromIndex(newValue)) :
                (ItemsSource is IList list ? list[newValue] : Items[newValue] );
        else
            PrimarySelectedItem = null;
        if (oldValue is >= 0 && SafeContainerFromIndex(oldValue) is UIElement container)
        {
            if (Canvas.GetZIndex(container) is 2)
                Canvas.SetZIndex(container, 0);
            if (container.FindDescendantOrSelf<MotionDragSelectableItem>() is { } item)
                item.IsPrimarySelected = false;
        }
        if (newValue is >= 0 && SafeContainerFromIndex(newValue) is UIElement container2)
        {
            if (Canvas.GetZIndex(container2) is 0)
                Canvas.SetZIndex(container2, 2);
            if (container2.FindDescendantOrSelf<MotionDragSelectableItem>() is { } item2)
                item2.IsPrimarySelected = true;
        }
        if (newValue is < 0 && PreferAlwaysSelectItem && ItemsCount > 0)
        {
            var guessNewIndex = Math.Clamp(oldValue - 1, 0, ItemsCount - 1);
            PrimarySelectedIndex = guessNewIndex;
        }
    }
    partial void OnPreferAlwaysSelectItemChanged(bool oldValue, bool newValue)
    {
        if (newValue && PrimarySelectedIndex < 0)
            // let's trigger PrimarySelectedIndex auto selection logic
            PrimarySelectedIndex = -1;
    }
    partial void OnPrimarySelectedItemChanged(object? oldValue, object? newValue)
    {
        if (oldValue == newValue) return;
        if (IsLoaded)
        {
            var container = SafeContainerFromIndex(PrimarySelectedIndex);
            if ((container is null && newValue is null) ||
                (container is not null && ItemFromContainer(container) == newValue))
            {
                // Everything is already taken care by index setter
                return;
            }
            if (newValue == null)
            {
                PrimarySelectedIndex = -1;
                return;
            }
            PrimarySelectedIndex = SafeIndexFromContainer(ContainerFromItem(newValue));
            return;
        }
        else
        {
            if (newValue is null)
            {
                if (PrimarySelectedIndex is < 0)
                    // this is already correct
                    return;
                PrimarySelectedIndex = -1;
            }
            if (ItemsSource is IList list)
            {
                if (PrimarySelectedIndex >= 0 && PrimarySelectedIndex < list.Count
                    && list[PrimarySelectedIndex] == newValue)
                    // this is already correct
                    return;
                var i = list.IndexOf(newValue);
                if (i > 0) throw new KeyNotFoundException("The item you requested to select is not found in the list");
                PrimarySelectedIndex = i;
            } else
            {
                if (PrimarySelectedIndex >= 0 && PrimarySelectedIndex < Items.Count
                    && Items[PrimarySelectedIndex] == newValue)
                    // this is already correct
                    return;
                var i = Items.IndexOf(newValue);
                if (i > 0) throw new KeyNotFoundException("The item you requested to select is not found in the list");
                PrimarySelectedIndex = i;
            }

        }
    }
    internal bool RequestPrimarySelect(MotionDragSelectableItem item)
    {
        if (!item.IsSelectableItemKind) throw new NotSupportedException();
        foreach (var child in item.FindAscendants())
        {
            var idx = IndexFromContainer(child);
            if (idx >= 0)
            {
                PrimarySelectedIndex = idx;
                return PrimarySelectedIndex == idx;
            }
        }
        return false;
    }
    internal bool RequestSelect(MotionDragSelectableItem item) => RequestSelect(item, false, false);
    internal bool RequestSelect(MotionDragSelectableItem item, bool shift, bool ctrl)
    {
        if (!item.IsSelectableItemKind) throw new NotSupportedException();
        if (shift is false && ctrl is false)
        {
            return RequestPrimarySelect(item);
        }
        return false;
    }
    TReturn? FirstNotNullOrNull<TReturn, TSource>(IEnumerable<TSource> t, Func<TSource, TReturn?> func) where TReturn : class
    {
        foreach (var item in t)
        {
            if (func(item) is TReturn output) return output;
        }
        return null;
    }
    internal bool IsPrimarySelected(MotionDragSelectableItem item)
    {
        return SafeContainerFromIndex(PrimarySelectedIndex)?
            .FindDescendantOrSelf<MotionDragSelectableItem>(
                x => x == item
            ) is not null;
    }
    internal void InternalRemoveItem(MotionDragSelectableItem item)
    {
        foreach (var child in item.FindAscendants())
        {
            var idx = IndexFromContainer(child);
            if (idx >= 0)
            {
                var itemSource = ItemsSource;
                if (itemSource is null)
                {
                    Items.RemoveAt(idx);
                }
                else if (itemSource is IList list)
                {
                    list.RemoveAt(idx);
                    if (list is not INotifyCollectionChanged)
                    {
                        // refresh ItemSource
                        ItemsSource = null;
                        ItemsSource = list;
                    }
                }
                else
                {
                    throw new NotSupportedException("ItemSource must implement IList");
                }
                return;
            }
        }
    }
    internal object? InternalGetItemAtIndex(int idx)
    {
        var itemSource = ItemsSource;
        if (itemSource is null)
        {
            return Items[idx];
        }
        else if (itemSource is IList list)
        {
            return list[idx];
        }
        else
        {
            throw new NotSupportedException("ItemSource must implement IList");
        }
    }
    int cachedPrimarySelectedIndex;
    protected override void OnItemDroppingFromAnotherContainer(object? sender, object? item, int senderIndex, int newIndex)
    {
        cachedPrimarySelectedIndex = PrimarySelectedIndex;
        PrimarySelectedIndex = -1;
        base.OnItemDroppingFromAnotherContainer(sender, item, senderIndex, newIndex);
    }
    protected override void OnItemDropFromAnotherContainer(object? sender, object? item, int senderIndex, int newIndex)
    {
        // adjust the current selection to the correct item
        if (newIndex <= cachedPrimarySelectedIndex)
            PrimarySelectedIndex = cachedPrimarySelectedIndex + 1;
        else
            PrimarySelectedIndex = cachedPrimarySelectedIndex;
        // let's see if we need to select the new item or not
        if (sender is ISelectableContainer other && other.PrimarySelectedIndex == senderIndex)
        {
            PrimarySelectedIndex = newIndex;
            other.PrimarySelectedItem = -1;
        }
        base.OnItemDropFromAnotherContainer(sender, item, senderIndex, newIndex);
    }
    int? newSelectionIndex = null;
    protected override void OnItemMovingInContainer(int oldIndex, int newIndex)
    {
        if (SafeContainerFromIndex(oldIndex) is { } a &&
            (a.FindDescendantOrSelf<MotionDragSelectableItem>()?.IsPrimarySelected ?? false))
        {
            newSelectionIndex = newIndex;
        } else
        {
            newSelectionIndex = null;
        }
        base.OnItemMovingInContainer(oldIndex, newIndex);
    }
    protected override void OnItemMovedInContainer(int oldIndex, int newIndex)
    {
        if (newSelectionIndex.HasValue)
            PrimarySelectedIndex = newIndex;
        base.OnItemMovedInContainer(oldIndex, newIndex);
    }
}