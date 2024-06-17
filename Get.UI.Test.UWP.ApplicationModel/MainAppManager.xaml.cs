using Get.UI.ApplicationModel.Tabbed;
using Get.UI.Controls.Tabs;
using Get.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
namespace Get.UI.Test.UWP.ApplicationModel;
partial class MainAppManager
{
    int i = 1;
    public MainAppManager()
    {
        InitializeComponent();
        var window = MakeTabWindowModel(WindowCoreWindow.CurrentThreadCoreWindow, CreateNewTab(null));
        window.Window.Activate();
        var root = (Page)global::Windows.UI.Xaml.Window.Current.Content;
        root.Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
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