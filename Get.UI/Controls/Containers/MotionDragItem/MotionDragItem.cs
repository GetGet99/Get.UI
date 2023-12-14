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
}
