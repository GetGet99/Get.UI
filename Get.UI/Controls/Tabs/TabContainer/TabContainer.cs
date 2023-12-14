using Get.UI.MotionDrag;

namespace Get.UI.Controls.Tabs;


[DependencyProperty<MotionDragConnectionContext>("ConnectionContext")]
[DependencyProperty<object>("Header")]
[DependencyProperty<DataTemplate>("HeaderTemplate")]
[DependencyProperty<object>("Footer")]
[DependencyProperty<DataTemplate>("FooterTemplate")]
[DependencyProperty<object>("InlineHeader")]
[DependencyProperty<DataTemplate>("InlineHeaderTemplate")]
[DependencyProperty<object>("InlineFooter")]
[DependencyProperty<DataTemplate>("InlineFooterTemplate")]
[DependencyProperty<double?>("TabContainerRequestedSize")]
[DependencyProperty<double>("TabSpacing")]
[DependencyProperty<Orientation>("Orientation", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<OrientationNeutralAlignment>("Alignment")]
[DependencyProperty<CenterAlignmentResolvingMode>("TabContainerCenterAlignmentResolvingMode")]
[DependencyProperty<double>("StartInset")]
[DependencyProperty<double>("EndInset")]
[DependencyProperty<object>("ItemsSource")]
[DependencyProperty<DataTemplate>("ItemTemplate")]
[DependencyProperty<object>("PrimarySelectedItem")]
public partial class TabContainer : Control
{
    public TabContainer()
    {
        DefaultStyleKey = typeof(TabContainer);
        ConnectionContext = new();
    }
    StackPanel StackPanelItemsPanel => (StackPanel)GetTemplateChild(nameof(StackPanelItemsPanel));
    partial void OnOrientationChanged(Orientation oldValue, Orientation newValue)
    {
        if (StackPanelItemsPanel is not { } sp) return;
        sp.Orientation = newValue;
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (StackPanelItemsPanel is not null)
            StackPanelItemsPanel.Orientation = Orientation;
    }
}