namespace Get.UI.Controls.Containers;
[DependencyProperty(typeof(object), "Content")]
[DependencyProperty(typeof(bool), "ReorderTranslationAnimation")]
[TemplatePart(Type = typeof(Border), Name = nameof(RootElement))]
[TemplatePart(Type = typeof(CompositeTransform), Name = nameof(RootTransform))]
[TemplatePart(Type = typeof(Storyboard), Name = nameof(TranslationResetStoryboard))]
[TemplatePart(Type = typeof(Storyboard), Name = nameof(AnimatingTranslationStoryboard))]
[TemplatePart(Type = typeof(DoubleAnimation), Name = nameof(AnimatingTranslateX))]
[TemplatePart(Type = typeof(DoubleAnimation), Name = nameof(AnimatingTranslateY))]
public partial class MotionDragItem : Control
{
    MotionDragContainer? Owner => this.FindAscendant<MotionDragContainer>();
    ///// <summary>
    ///// Tells whether this tab kind is supposed to be selectable.
    ///// Useful if you want to create spacial kind of tab where
    ///// the tab itself is supposed to be used as a display-only
    ///// and do not supposed to act as a tab.
    ///// </summary>
    //public virtual bool IsSelectableTabKind => true;
    //public virtual bool IsTabGroupKind => false;

    public MotionDragItem()
    {
        DefaultStyleKey = typeof(MotionDragItem);
        // default property
        ReorderTranslationAnimation = true;
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RootElement.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        InitManipulation();
    }
    [Property(Visibility = GeneratorVisibility.Protected, SetVisibility = GeneratorVisibility.Private, OnChanged = nameof(OnPointerVisualStateUpdated))]
    PointerVisualState _CurrentPointerVisualState;
    protected virtual void OnPointerVisualStateUpdated()
    {
        VisualStateManager.GoToState(this, CurrentPointerVisualState.ToString(), true);
    }
    bool isPointerInside = false;
    bool isPressed = false;
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        isPointerInside = true;
        UpdateState();
        base.OnPointerEntered(e);
    }
    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        isPointerInside = false;
        UpdateState();
        base.OnPointerExited(e);
    }
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        isPressed = true;
        UpdateState();
        base.OnPointerPressed(e);
    }
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        isPressed = false;
        UpdateState();
        base.OnPointerReleased(e);
    }
    void UpdateState()
    {

        if (isPressed) CurrentPointerVisualState = PointerVisualState.Pressed;
        else
        {
            if (isPointerInside)
                CurrentPointerVisualState = PointerVisualState.PointerOver;
            else
                CurrentPointerVisualState = PointerVisualState.Normal;
        }
    }
    protected enum PointerVisualState
    {
        Normal,
        PointerOver,
        Pressed
    }
}
