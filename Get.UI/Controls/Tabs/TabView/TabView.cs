using Get.UI.MotionDrag;

namespace Get.UI.Controls.Tabs;

[DependencyProperty(typeof(bool), "AllowUserTabGroupping", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(Visibility), "TabsVisibility", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(TabAlignment), "TabsAlignment", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(Visibility), "ToolbarVisibility", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(MotionDragConnectionContext), "ConnectionContext")]
[DependencyProperty(typeof(object), "ItemsSource", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(object), "PrimarySelectedItem", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "ItemTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "ToolbarTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "ContentTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabHeaderTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabFooterTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabInlineHeaderTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabInlineFooterTemplate", GenerateLocalOnPropertyChangedMethod = true)]
public partial class TabView : Control
{
    public TabView()
    {
        DefaultStyleKey = typeof(TabView);
        ConnectionContext = new();
    }
}
public enum TabAlignment
{
    // Left or Top
    Start,
    Center,
    // Bottom or Right
    End
}