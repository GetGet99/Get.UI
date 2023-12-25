using Get.UI.MotionDrag;

namespace Get.UI.Controls.Tabs;

[DependencyProperty(typeof(bool), "AllowUserTabGroupping", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(Visibility), "TabsVisibility", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(TabAlignment), "TabsAlignment", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(Visibility), "ToolbarVisibility", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(MotionDragConnectionContext), "ConnectionContext")]
[DependencyProperty(typeof(object), "ItemsSource", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(object), "PrimarySelectedItem", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(int), "PrimarySelectedIndex", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "ItemTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "ToolbarTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "ContentTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabHeaderTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabFooterTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabInlineHeaderTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(DataTemplate), "TabInlineFooterTemplate", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(double), "TitleBarLeftInset", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty(typeof(double), "TitleBarRightInset", GenerateLocalOnPropertyChangedMethod = true)]
public partial class TabView : Control
{
    public TabView()
    {
        DefaultStyleKey = typeof(TabView);
        ConnectionContext = new();
    }
    T GetTemplateChild<T>(string name) where T : DependencyObject => (T)GetTemplateChild(name);
    ContentPresenter? ContentPresenter => GetTemplateChild<ContentPresenter>(nameof(ContentPresenter));
    ContentPresenter? ToolbarPresenter => GetTemplateChild<ContentPresenter>(nameof(ToolbarPresenter));

    partial void OnPrimarySelectedItemChanged(object oldValue, object newValue)
    {
        if (ContentPresenter != null && ToolbarPresenter != null)
            ContentPresenter.Visibility = ToolbarPresenter.Visibility
                = newValue is null ? Visibility.Collapsed : Visibility.Visible;
    }
    protected override void OnApplyTemplate()
    {
        var newValue = PrimarySelectedItem;
        ContentPresenter.Visibility = ToolbarPresenter.Visibility
            = newValue is null ? Visibility.Collapsed : Visibility.Visible;
        base.OnApplyTemplate();
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