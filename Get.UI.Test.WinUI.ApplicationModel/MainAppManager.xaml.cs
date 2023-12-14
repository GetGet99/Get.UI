using Get.UI.ApplicationModel.Tabbed;
namespace Get.UI.Test.WinUI.ApplicationModel;
partial class MainAppManager
{
    public MainAppManager()
    {
        InitializeComponent();
        var window = CreateNewWindow();
        for (int i = 0; i < 10; i++)
        {
            window.TabItems.Add($"Item {i + 1}");
        }
        window.Window.PlatformWindow.Activate();
    }
}
class TypedAppManager : TabbedApplicationManager<string> { }