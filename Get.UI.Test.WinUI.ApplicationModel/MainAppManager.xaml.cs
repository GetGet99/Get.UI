using Get.UI.ApplicationModel.Tabbed;
using Get.UI.Controls.Tabs;
using Get.UI.Windowing;
using System;
using System.Windows.Input;
namespace Get.UI.Test.WinUI.ApplicationModel;
partial class MainAppManager
{
    int i = 1;
    public MainAppManager()
    {
        InitializeComponent();
        var window = CreateNewWindow(CreateNewTab(null));
        window.Window.PlatformWindow.Activate();
    }
    protected override void OnWindowClosing(Window window)
    {
        
    }
    protected override string CreateNewTab(object? parameter)
    {
        return $"Item {i++}";
    }
}
class SimpleCloseCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter)
    {
        if (parameter is TabClosingHandledEventArgs e)
        {
            e.Handled = true;
            e.RemoveRequest = true;
        }
    }
}
abstract class TypedAppManager : TabbedApplicationManager<string> { }