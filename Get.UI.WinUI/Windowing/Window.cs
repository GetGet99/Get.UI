using Microsoft.UI.Windowing;
using System.Drawing;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Get.UI.Windowing;

public class WindowAppWindow : Window
{
    bool IsSettingRootContentAllowed;
    public AppWindow AppWindow { get; }
    public override object PlatformWindow { get; }
    public override UIElement RootContent
    {
        get => XamlRoot.Content;
        set => SelfNote.ThrowNotImplemented();
    }
    public override XamlRoot XamlRoot { get; }
    public override Rect Bounds
    {
        get
        {
            var (pt, size) = (AppWindow.Position, AppWindow.Size);
            return new(pt.X, pt.Y, size.Width, size.Height);
        }
        set
        {
            AppWindow.MoveAndResize(new((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height));
        }
    }
    internal WindowAppWindow(XamlRoot xamlRoot)
    {
        PlatformWindow = AppWindow = AppWindow.GetFromWindowId(xamlRoot.ContentIslandEnvironment.AppWindowId);
        XamlRoot = xamlRoot;
    }
    public override void Close() => WinWrapper.Windowing.Window.FromWindowHandle(WindowHandle).TryClose();
    public override double TitleBarLeftInset => AppWindow.TitleBar.LeftInset;
    public override double TitleBarRightInset => AppWindow.TitleBar.RightInset;

    public override Rect ClientBounds { get => SelfNote.ThrowNotImplemented<Rect>(); set => SelfNote.ThrowNotImplemented<Rect>(); }

    public override nint WindowHandle => (nint)AppWindow.Id.Value;
    public override bool ExtendsContentIntoTitleBar { get => AppWindow.TitleBar.ExtendsContentIntoTitleBar; set => AppWindow.TitleBar.ExtendsContentIntoTitleBar = value; }
    EventHandler? _Closed;
    DefferalCanceledEventHandler? _Closing;
    public override event EventHandler Closed
    {
        add
        {
            _Closed += value;
            if (_Closed is not null || _Closing is not null)
            {
                AppWindow.Closing -= AppWindow_Closing;
                AppWindow.Closing += AppWindow_Closing;
            }
        }
        remove
        {
            _Closed -= value;
            if (_Closed is not null && _Closing is not null)
            {
                AppWindow.Closing -= AppWindow_Closing;
            }
        }
    }
    bool isForcedClosed = false;
    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs e)
    {
        if (isForcedClosed)
        {
            _Closed?.Invoke(this, new());
            return;
        }
        e.Cancel = true;
        DefferalCanceledEventArgs args = new();
        _Closing?.Invoke(this, args);
        await (args.DeferralInternal?.WaitAsync() ?? Task.CompletedTask);
        if (args.Cancel) return;
        isForcedClosed = true;
        Close();
    }
    public override void Activate()
    {
        AppWindow.Show();
    }

    public override event DefferalCanceledEventHandler Closing
    {
        add
        {
            _Closing += value;
            if (_Closed is not null || _Closing is not null)
            {
                AppWindow.Closing -= AppWindow_Closing;
                AppWindow.Closing += AppWindow_Closing;
            }
        }
        remove
        {
            _Closing -= value;
            if (_Closed is not null && _Closing is not null)
            {
                AppWindow.Closing -= AppWindow_Closing;
            }
        }
    }
}
public class WindowXAMLWindow : WindowAppWindow
{
    bool IsSettingRootContentAllowed;
    public override Microsoft.UI.Xaml.Window PlatformWindow { get; }
    public override UIElement RootContent
    {
        get => base.RootContent;
        set => PlatformWindow.Content = IsSettingRootContentAllowed ? value : throw new NotSupportedException();
    }
    internal WindowXAMLWindow(Microsoft.UI.Xaml.Window window, bool isSettingRootContentAllowed = true)
        : base(window.Content.XamlRoot)
    {
        PlatformWindow = window;
        IsSettingRootContentAllowed = isSettingRootContentAllowed;
    }
}
partial class Window
{
    public static Window GetFromXamlRoot(XamlRoot root)
    {
        return new WindowAppWindow(root);
    }
    public static Window GetFromPlatformWindow(Microsoft.UI.Xaml.Window window, bool isSettingRootContentAllowed = true)
    {
        return new WindowXAMLWindow(window, isSettingRootContentAllowed);
    }
    public static Window Create()
    {
        return GetFromPlatformWindow(new());
    }
    public static Task<Window> CreateAsync() => Task.FromResult(Create());
}