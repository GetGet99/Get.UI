using System.Threading.Tasks;
using Windows.Internal;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using WinRT;

namespace Get.UI.Windowing;

partial class Window
{
    static object GetWindowContext(XamlRoot root)
    {
        return root.UIContext.As<IUIContextPartner>().WindowContext;
    }
    public static Window GetFromXamlRoot(XamlRoot root)
    {
        var platformWindow = GetWindowContext(root);
        if (platformWindow is CoreWindow cw)
            return new WindowCoreWindow(cw, root);
        else if (platformWindow is AppWindow aw)
            return new WindowAppWindow(aw, root);
        else
            throw new NotSupportedException();
    }
    public static async Task<Window> CreateAsync()
    {
        return await CreateAppWindowAsync();
    }
    public static async Task<WindowAppWindow> CreateAppWindowAsync()
    {
        var appwindow = await AppWindow.TryCreateAsync();
        ElementCompositionPreview.SetAppWindowContent(appwindow, new Grid { });
        return new(appwindow, ElementCompositionPreview.GetAppWindowContent(appwindow).XamlRoot);
    }
}