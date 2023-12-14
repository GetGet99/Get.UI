#nullable enable
namespace Get.UI.Controls.Containers;
[DependencyProperty(typeof(object), "PrimarySelectedItem", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
[DependencyProperty(typeof(int), "PrimarySelectedIndex", GenerateLocalOnPropertyChangedMethod = true)]
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
    }
    object? selectedItemCache;

    partial void OnPrimarySelectedItemChanged(object? oldValue, object? newValue)
    {
        if (oldValue is not null && ContainerFromItem(oldValue) is UIElement container)
        {
            if (Canvas.GetZIndex(container) is 2)
                Canvas.SetZIndex(container, 0);
            if (container.FindDescendantOrSelf<MotionDragSelectableItem>() is { } item)
                item.IsPrimarySelected = false;
        }
        if (newValue is not null && ContainerFromItem(newValue) is UIElement container2)
        {
            if (Canvas.GetZIndex(container2) is 0)
                Canvas.SetZIndex(container2, 2);
            if (container2.FindDescendantOrSelf<MotionDragSelectableItem>() is { } item2)
                item2.IsPrimarySelected = true;
        }
    }
    internal bool RequestPrimarySelect(MotionDragSelectableItem item)
    {
        if (!item.IsSelectableItemKind) throw new NotSupportedException();
        var curObj = FirstNotNullOrNull(item.FindAscendants(), x => ItemFromContainer(x));
        PrimarySelectedItem = curObj;
        return PrimarySelectedItem == curObj;
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
        return ContainerFromItem(PrimarySelectedItem)?
            .FindDescendantOrSelf<MotionDragSelectableItem>(
                x => x == item
            ) is not null;
    }
    protected override void OnItemDropFromAnotherContainer(object? sender, object? item)
    {
        if (sender is ISelectableContainer other && other.PrimarySelectedItem == item)
        {
            PrimarySelectedItem = item;
            other.PrimarySelectedItem = null;
        }
        base.OnItemDropFromAnotherContainer(sender, item);
    }
}