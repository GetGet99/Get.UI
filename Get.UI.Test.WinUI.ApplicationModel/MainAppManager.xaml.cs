using CommunityToolkit.WinUI;
using Get.UI.ApplicationModel.Tabbed;
using Get.UI.Controls.Tabs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;
namespace Get.UI.Test.WinUI.ApplicationModel;
partial class MainAppManager
{
    int i = 1;
    public MainAppManager()
    {
        InitializeComponent();
        InitAsync();
    }
    async void InitAsync()
    {
        var window = await CreateNewWindowAsync(CreateNewTab(null));
        window.Window.Activate();
    }
    protected override void OnWindowClosing(Windowing.Window window)
    {
        
    }
    protected override string CreateNewTab(object? parameter)
    {
        return $"Item {i++}";
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var tabView = (sender as DependencyObject)?.FindAscendant<Get.UI.Controls.Tabs.TabView>();
        if (tabView is null) return;
        tabView.Orientation = tabView.Orientation is Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical;
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