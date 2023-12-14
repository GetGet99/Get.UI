#nullable enable
namespace Get.UI.Controls;
[DependencyProperty(typeof(Orientation), "Orientation", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodWithParameter = false, LocalOnPropertyChangedMethodName = "Update")]
[DependencyProperty(typeof(double), "OffsetAmount", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodWithParameter = false, LocalOnPropertyChangedMethodName = "Update")]
[TemplatePart(Type = typeof(FrameworkElement), Name = nameof(RootElement))]
public partial class OffsetElement : Control
{
    FrameworkElement? RootElement => (FrameworkElement)GetTemplateChild(nameof(RootElement));
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        Update();
    }
    void Update()
    {
        if (RootElement is { } ele)
        {
            if (Orientation is Orientation.Horizontal)
                (ele.Width, ele.Height) = (OffsetAmount, 1);
            else
                (ele.Width, ele.Height) = (1, OffsetAmount);
        }
    }
}