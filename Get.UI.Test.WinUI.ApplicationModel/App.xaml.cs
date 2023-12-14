using Microsoft.UI.Xaml;

namespace Get.UI.Test.WinUI.ApplicationModel;

public partial class App : Application
{
    MainAppManager ApplicationManager;
    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        ApplicationManager = new MainAppManager();
    }
}
