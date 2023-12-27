using Get.UI.MotionDrag;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Get.UI.Controls.Tabs;

[DependencyProperty<object>("Header")]
[DependencyProperty<DataTemplate>("HeaderTemplate")]
[DependencyProperty<object>("Footer")]
[DependencyProperty<DataTemplate>("FooterTemplate")]
[DependencyProperty<object>("InlineHeader")]
[DependencyProperty<DataTemplate>("InlineHeaderTemplate")]
[DependencyProperty<object>("InlineFooter")]
[DependencyProperty<DataTemplate>("InlineFooterTemplate")]
[DependencyProperty<double?>("TabContainerRequestedSize")]
[DependencyProperty<Orientation>("Orientation", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<OrientationNeutralAlignment>("Alignment")]
[DependencyProperty<CenterAlignmentResolvingMode>("TabContainerCenterAlignmentResolvingMode")]
[DependencyProperty<object>("ItemsSource")]
[DependencyProperty<DataTemplate>("ItemTemplate")]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext")]
[DependencyProperty<object>("PrimarySelectedItem", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<int>("PrimarySelectedIndex")]
[DependencyProperty<double>("TitleBarLeftInset")]
[DependencyProperty<double>("TitleBarRightInset")]
[DependencyProperty<ICommand>("TabCloseCommand", UseNullableReferenceType = true)]
[DependencyProperty<Visibility>("TabsVisibility")]
[DependencyProperty<DataTemplate>("ToolbarTemplate")]
[DependencyProperty<DataTemplate>("ContentTemplate")]
[DependencyProperty<Visibility>("AddTabButtonVisibility")]
[DependencyProperty<ICommand>("AddTabCommand", UseNullableReferenceType = true)]
[DependencyProperty<object>("AddTabCommandParameter", UseNullableReferenceType = true)]
//[DependencyProperty<bool>("AllowUserTabGroupping")]
//[DependencyProperty<TabAlignment>("TabsAlignment")]
//[DependencyProperty<Visibility>("ToolbarVisibility")]
public partial class TabView : Control
{
    public TabView()
    {
        DefaultStyleKey = typeof(TabView);
        ConnectionContext = new();
    }
    public event RoutedEventHandler? AddTabButtonClicked;
    T GetTemplateChild<T>(string name) where T : DependencyObject => (T)GetTemplateChild(name);
    ContentPresenter? ContentPresenter => GetTemplateChild<ContentPresenter>(nameof(ContentPresenter));
    ContentPresenter? ToolbarPresenter => GetTemplateChild<ContentPresenter>(nameof(ToolbarPresenter));
    TabContainer? TabContainer => GetTemplateChild<TabContainer>(nameof(TabContainer));

    partial void OnPrimarySelectedItemChanged(object oldValue, object newValue)
    {
        if (ContentPresenter != null && ToolbarPresenter != null)
            ContentPresenter.Visibility = ToolbarPresenter.Visibility
                = newValue is null ? Visibility.Collapsed : Visibility.Visible;
    }
    protected override void OnApplyTemplate()
    {
        var newValue = PrimarySelectedItem;
        ContentPresenter!.Visibility = ToolbarPresenter!.Visibility
            = newValue is null ? Visibility.Collapsed : Visibility.Visible;
        TabContainer!.AddTabButtonClicked += (o, e) => AddTabButtonClicked?.Invoke(o, e);
        base.OnApplyTemplate();
    }
    public Task<bool> AttemptToCloseAllTabsAsync() => TabContainer?.AttemptToCloseAllTabsAsync() ?? Task.FromResult(false);
}
public enum TabAlignment
{
    /// <summary>
    /// Left or Top
    /// </summary>
    Start,
    Center,
    /// <summary>
    /// Bottom or Right
    /// </summary>
    End
}