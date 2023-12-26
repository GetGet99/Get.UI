#nullable enable
using Get.UI.Controls.Containers;
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
[DependencyProperty<double>("StartInset")]
[DependencyProperty<double>("EndInset")]
[DependencyProperty<object>("ItemsSource")]
[DependencyProperty<DataTemplate>("ItemTemplate")]
[DependencyProperty<object>("PrimarySelectedItem")]
[DependencyProperty<int>("PrimarySelectedIndex")]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext")]
//[DependencyProperty<double>("TabSpacing")]
[DependencyProperty<ICommand>("TabCloseCommand", UseNullableReferenceType = true)]
[DependencyProperty<Visibility>("AddTabButtonVisibility")]
[DependencyProperty<ICommand>("AddTabCommand", UseNullableReferenceType = true, GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<object>("AddTabCommandParameter", UseNullableReferenceType = true)]
public partial class TabContainer : Control
{
    public event RoutedEventHandler? AddTabButtonClicked;
    public event EventHandler<TabClosedRequestEventArgs> TabCloseRequest;
    public TabContainer()
    {
        DefaultStyleKey = typeof(TabContainer);
        ConnectionContext = new();
        TabCloseRequest += (o, e) =>
        {
            if (TabCloseCommand is not null && TabCloseCommand.CanExecute(e))
                TabCloseCommand.Execute(e);
        };
    }
    MotionDragSelectableContainer Container => (MotionDragSelectableContainer)GetTemplateChild(nameof(Container));
    StackPanel StackPanelItemsPanel => (StackPanel)GetTemplateChild(nameof(StackPanelItemsPanel));
    Button AddTabButton => (Button)GetTemplateChild(nameof(AddTabButton));
    ScrollViewer ContainerAreaScrollViewer => (ScrollViewer)GetTemplateChild(nameof(ContainerAreaScrollViewer));
    partial void OnOrientationChanged(Orientation oldValue, Orientation newValue)
    {
        if (StackPanelItemsPanel is not { } sp) goto nextCategory;
        sp.Orientation = newValue;
    nextCategory:
        UpdateScrollView(newValue);
    }
    void UpdateScrollView(Orientation newValue)
    {
        if (ContainerAreaScrollViewer is { } sv)
        {
            if (newValue is Orientation.Horizontal)
            {
                sv.HorizontalScrollMode = ScrollMode.Auto;
                sv.VerticalScrollMode = ScrollMode.Disabled;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.VerticalScrollBarVisibility =  ScrollBarVisibility.Hidden;
            } else
            {
                sv.VerticalScrollMode = ScrollMode.Auto;
                sv.HorizontalScrollMode = ScrollMode.Disabled;
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (StackPanelItemsPanel is not null)
            StackPanelItemsPanel.Orientation = Orientation;
        AddTabButton.Click += (o, e) => AddTabButtonClicked?.Invoke(o, e);
        AddTabButton.Command = AddTabCommand;
        UpdateScrollView(Orientation);
    }
    partial void OnAddTabCommandChanged(ICommand? oldValue, ICommand? newValue)
    {
        // Template binding doesn't work for some reason so doing this manually
        if (AddTabButton is not null)
            AddTabButton.Command = newValue;
    }
    internal async Task<bool> InternalCallTabCloseRequestAsync(TabItem tabItem)
    {
        TabClosedRequestEventArgs args = new(tabItem, Container.ObjectFromSRI(tabItem));
        TabCloseRequest?.Invoke(this, args);
        await args.InternalWaitAsync();
        if (args.RemoveRequest)
        {
            Container.InternalRemoveItem(tabItem);
            return true;
        }
        return false;
    }
    public async Task<bool> AttemptToCloseAllTabsAsync()
    {
        while (Container.SafeContainerFromIndex(0) is { } a &&
            a.FindDescendantOrSelf<TabItem>() is { } tabItem)
        {
            var item = Container.InternalGetItemAtIndex(0);
            if (!await tabItem.InternalAttemptToCloseAsync())
            {
                // check if the library consumer removes item from container themselves or not
                if (ReferenceEquals(item, Container.InternalGetItemAtIndex(0)))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
public class TabClosedRequestEventArgs
{
    internal TabClosedRequestEventArgs(TabItem Tab, object? Item)
    {
        this.Tab = Tab;
        this.Item = Item;
    }
    public TabItem Tab { get; }
    public object? Item { get; }
    public bool RemoveRequest { get; set; } = false;
    Deferral? def;
    public Deferral GetDeferal() => def ??= new();
    internal Task InternalWaitAsync() => def?.WaitAsync() ?? Task.CompletedTask;
}
