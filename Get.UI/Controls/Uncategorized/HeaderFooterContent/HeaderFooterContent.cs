using Get.UI.Controls.Internals;

namespace Get.UI.Controls;

[DependencyProperty<object>("Header")]
[DependencyProperty<DataTemplate>("HeaderTemplate")]
[DependencyProperty<object>("Footer")]
[DependencyProperty<DataTemplate>("FooterTemplate")]
[DependencyProperty<object>("InlineHeader")]
[DependencyProperty<DataTemplate>("InlineHeaderTemplate")]
[DependencyProperty<object>("Content")]
[DependencyProperty<DataTemplate>("ContentTemplate")]
[DependencyProperty<object>("InlineFooter")]
[DependencyProperty<DataTemplate>("InlineFooterTemplate")]
[DependencyProperty<double?>("ContentRequestedSize", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdatePanel), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty(typeof(Orientation), "Orientation", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdatePanel), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty(typeof(OrientationNeutralAlignment), "Alignment", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdatePanel), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty(typeof(CenterAlignmentResolvingMode), "CenterAlignmentResolvingMode", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodName = nameof(UpdatePanel), LocalOnPropertyChangedMethodWithParameter = false)]
[DependencyProperty(typeof(double), "StartInset", Documentation = "/// <summary>The start offset for footer. Note that this does not effect the centered element.</summary>")]
[DependencyProperty(typeof(double), "EndInset", Documentation = "/// <summary>The end offset for footer. Note that this does not effect the centered element.</summary>")]
[TemplatePart(Type = typeof(UIElement), Name = "HeaderAndInset")]
[TemplatePart(Type = typeof(UIElement), Name = "ContentAndInline")]
[TemplatePart(Type = typeof(UIElement), Name = "FooterAndInset")]
[TemplatePart(Type = typeof(CustomizablePanel), Name = "PanelHost")]
[ContentProperty(Name = nameof(Content))]
public partial class HeaderFooterContent : Control
{
    void UpdatePanel()
    {
        PanelHost?.InvalidateMeasure();
        PanelHost?.InvalidateArrange();
        PanelHost?.UpdateLayout();
    }
}
public enum CenterAlignmentResolvingMode
{
    /// <summary>
    /// Force the element to center no matter what happended. The element resizes down to still keep centering.
    /// </summary>
    AbsoluteCenterResizeDown,
    /// <summary>
    /// If there are still space left but can no longer be centered, the element will align left or right to keep the current size first and resize down only when necessary.
    /// </summary>
    AlignLeftOrRight
}