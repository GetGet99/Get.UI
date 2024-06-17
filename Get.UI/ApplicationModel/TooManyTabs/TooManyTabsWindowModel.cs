using Get.UI.ApplicationModel.Tabbed;
using System.Collections.ObjectModel;
using Window = Get.UI.Windowing.Window;
namespace Get.UI.ApplicationModel.TooManyTabs;

public partial class TooManyTabsWindowModel<T>
{
    internal TooManyTabsWindowModel(TabbedApplicationManager<T> owner, Window window)
    {
        Manager = owner;
        Window = window;
    }
    public TabbedApplicationManager<T> Manager { get; }
    public Window Window { get; }
    public ObservableCollection<T> TabItems { get; } = [];
}