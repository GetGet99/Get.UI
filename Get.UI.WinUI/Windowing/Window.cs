using System.Drawing;

namespace Get.UI.Windowing;

public class Window
{
    bool IsSettingRootContentAllowed;
    public Microsoft.UI.Xaml.Window PlatformWindow { get; }
    public UIElement RootContent
    {
        get => PlatformWindow.Content;
        set => PlatformWindow.Content = IsSettingRootContentAllowed ? value : throw new NotSupportedException();
    }
    public XamlRoot XamlRoot => PlatformWindow.Content.XamlRoot;
    public Rectangle Bounds
    {
        get
        {
            var (pt, size) = (PlatformWindow.AppWindow.Position, PlatformWindow.AppWindow.Size);
            return new(pt.X, pt.Y, size.Width, size.Height);
        }
        set
        {
            PlatformWindow.AppWindow.MoveAndResize(new(value.X, value.Y, value.Width, value.Height));
        }
    }
    internal Window(Microsoft.UI.Xaml.Window window, bool isSettingRootContentAllowed = true)
    {
        PlatformWindow = window;
        IsSettingRootContentAllowed = isSettingRootContentAllowed;
    }
    public void Close() => PlatformWindow.Close();
    public int TitleBarLeftInset => PlatformWindow.AppWindow.TitleBar.LeftInset;
    public int TitleBarRightInset => PlatformWindow.AppWindow.TitleBar.LeftInset;
}