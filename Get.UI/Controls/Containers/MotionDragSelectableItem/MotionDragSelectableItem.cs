namespace Get.UI.Controls.Containers;

[DependencyProperty(typeof(bool), "IsSelected", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(bool), "IsPrimarySelected", GenerateLocalOnPropertyChangedMethod = true)]
[TemplateVisualState(GroupName = "Selection", Name = "Selected")]
[TemplateVisualState(GroupName = "Selection", Name = "Deselected")]
[TemplateVisualState(GroupName = "PrimarySelection", Name = "PrimarySelected")]
[TemplateVisualState(GroupName = "PrimarySelection", Name = "PrimaryDeselected")]
[TemplateVisualState(GroupName = "MultipleSelection", Name = "MultipleSelected")]
[TemplateVisualState(GroupName = "MultipleSelection", Name = "MultipleDeselected")]
public partial class MotionDragSelectableItem : MotionDragItem
{
    MotionDragSelectableContainer? Owner => this.FindAscendant<MotionDragSelectableContainer>();
    /// <summary>
    /// Tells whether this item kind is supposed to be selectable.
    /// </summary>
    // Useful if you want to create spacial kind of tab where
    // the tab itself is supposed to be used as a display-only
    // and do not supposed to act as a tab.
    internal protected virtual bool IsSelectableItemKind => true;
    public MotionDragSelectableItem()
    {
        Loaded += MotionDragSelectableItem_Loaded;
    }

    private void MotionDragSelectableItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsPrimarySelectedAccordingToOwner ?? false)
        {
            IsPrimarySelected = true;
        }
    }
    bool? IsPrimarySelectedAccordingToOwner
        => Owner?.IsPrimarySelected(this);
    partial void OnIsPrimarySelectedChanged(bool oldValue, bool newValue)
    {
        if (oldValue == newValue) return;
        if (newValue) // Selected
        {
            if (!IsSelectableItemKind) throw new NotSupportedException($"Selection is not supported because {nameof(IsSelectableItemKind)} is false");
            if (Owner is { } owner)
            {

                if (!IsPrimarySelectedAccordingToOwner ?? false)
                {
                    if (!owner.RequestPrimarySelect(this))
                    {
                        throw new InvalidOperationException("Cannot set the currently selected item to the current item");
                    }
                }
            }
        }
        // Mirror IsSelected for now
        IsSelected = newValue;
        OnPointerVisualStateUpdated();
    }
    protected override void OnPointerVisualStateUpdated()
    {
        if (!IsSelected)
            base.OnPointerVisualStateUpdated();
        else
        {
            if (CurrentPointerVisualState is PointerVisualState.Normal)
                VisualStateManager.GoToState(this, "Selected", true);
            else
                VisualStateManager.GoToState(this, $"{CurrentPointerVisualState}Selected", true);
        }
    }
    partial void OnIsSelectedChanged(bool oldValue, bool newValue)
    {
        // Mirror IsPrimarySelected for now
        IsPrimarySelected = newValue;
    }
    //protected override void OnPointerPressed(PointerRoutedEventArgs e)
    //{
    //    base.OnPointerPressed(e);
    //    if (!IsSelectableItemKind) return;
    //    if (!e.Handled)
    //    {
    //        e.Handled = true;
    //        bool shift = e.KeyModifiers.HasFlag(VirtualKeyModifiers.Shift);
    //        bool ctrl = e.KeyModifiers.HasFlag(VirtualKeyModifiers.Shift);
    //        IsPrimarySelected = true;
    //    }
    //}
    protected override void OnTapped(TappedRoutedEventArgs e)
    {
        base.OnTapped(e);
        if (!IsSelectableItemKind) return;
        if (!e.Handled)
        {
            e.Handled = true;
            //bool shift = e.KeyModifiers.HasFlag(VirtualKeyModifiers.Shift);
            //bool ctrl = e.KeyModifiers.HasFlag(VirtualKeyModifiers.Shift);
            IsPrimarySelected = true;
        }
    }
    T FindName<T>(string name) where T : DependencyObject => (T)GetTemplateChild(name);
    Border? RootElement => FindName<Border>(nameof(RootElement));

}