using Get.UI.Controls.Containers;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Get.UI.Controls.Tabs;
[DependencyProperty<ICommand>("CloseButtonCommand", UseNullableReferenceType = true)]
public partial class TabItem : MotionDragSelectableItem
{
    Button CloseButton => (Button)GetTemplateChild(nameof(CloseButton));
    public event EventHandler<TabClosingHandledEventArgs> Closing;
    public TabItem()
    {
        DefaultStyleKey = typeof(TabItem);
        Closing += (_, e) =>
        {
            if (CloseButtonCommand is not null && CloseButtonCommand.CanExecute(e))
                CloseButtonCommand.Execute(e);
        };
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        CloseButton.Click += (_, _) => _ = InternalAttemptToCloseAsync();
    }

    MotionDragSelectableContainer? OwnerContainer => this.FindAscendant<MotionDragSelectableContainer>();
    TabContainer? Owner => this.FindAscendant<TabContainer>();
    internal async Task<bool> InternalAttemptToCloseAsync()
    {
        var cmd = CloseButtonCommand;
        TabClosingHandledEventArgs args = new();
        Closing(this, args);
        await args.InternalWaitAsync();
        if (args.Handled)
        {
            if (args.RemoveRequest)
            {
                OwnerContainer?.InternalRemoveItem(this);
                return true;
            } else return false;
        }
        return await (Owner?.InternalCallTabCloseRequestAsync(this) ?? Task.FromResult(false));
    }
}
public class TabClosingHandledEventArgs
{
    internal TabClosingHandledEventArgs() { }
    public bool Handled { get; set; } = false;
    public bool RemoveRequest { get; set; } = false;
    Deferral? def;
    public Deferral GetDeferal() => def ??= new();
    internal Task InternalWaitAsync() => def?.WaitAsync() ?? Task.CompletedTask;
}