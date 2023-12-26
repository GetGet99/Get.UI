using Get.UI.ApplicationModel.Tabbed;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Window = Get.UI.Windowing.Window;
namespace Get.UI.ApplicationModel.Tabbed;

public class TabbedWindowModel<T> : TabbedWindowModel
{
    internal TabbedWindowModel(TabbedApplicationManager<T> owner, Window window) : base(owner, window, new ObservableCollection<T>())
    {
        Manager = owner;
        TabItems = (ObservableCollection<T>)base.TabItems;
    }
    public new TabbedApplicationManager<T> Manager { get; }
    public new ObservableCollection<T> TabItems { get; }
}
[DependencyProperty<int>("PrimarySelectedIndex")]
public partial class TabbedWindowModel : DependencyObject
{
    internal TabbedWindowModel(TabbedApplicationManager owner, Window window, object tabItems)
    {
        Manager = owner;
        Window = window;
        TabItems = tabItems;
        AddTabCommand = new ActionCommand(x => Manager.InternalAddTab(this, x));
    }

    public TabbedApplicationManager Manager { get; }
    public Window Window { get; }
    public object TabItems { get; }
    public ICommand AddTabCommand { get; }
}