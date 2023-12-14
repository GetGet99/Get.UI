namespace Get.UI.Controls.Internals;

/// <summary>
/// Class for internal logic of Get.UI.
/// This class is exposed as public for those who want to customize control template.
/// </summary>
public sealed class CustomizablePanel : Panel
{
    internal delegate Size MeasureHandler(Size availableSize);
    internal event MeasureHandler? OnMeasure;
    internal delegate Size ArrangeHandler(Size finalSize);
    internal event ArrangeHandler? OnArrange;

    protected override Size MeasureOverride(Size availableSize)
        => OnMeasure?.Invoke(availableSize) ?? base.MeasureOverride(availableSize);
    protected override Size ArrangeOverride(Size finalSize)
        => OnArrange?.Invoke(finalSize) ?? base.ArrangeOverride(finalSize);
}