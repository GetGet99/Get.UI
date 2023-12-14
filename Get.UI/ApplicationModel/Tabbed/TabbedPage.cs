namespace Get.UI.ApplicationModel.Tabbed;

internal sealed partial class TabbedPage : Page
{
    readonly TabbedApplicationManager TabbedApplication;
    readonly TabbedWindowModel WindowModel;
    public TabbedPage(TabbedApplicationManager tabbedApplication, TabbedWindowModel model)
    {
        TabbedApplication = tabbedApplication;
        WindowModel = model;
        InitializeComponent();
    }
}