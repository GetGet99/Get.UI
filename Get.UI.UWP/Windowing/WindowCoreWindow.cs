using System.Drawing;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Internal;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using WinRT;

namespace Get.UI.Windowing;

public class WindowCoreWindow : Window
{
    public static WindowCoreWindow CurrentThreadCoreWindow => new(
        CoreWindow.GetForCurrentThread(),
        Windows.UI.Xaml.Window.Current.Content.XamlRoot
    );
    readonly CoreWindow window;
    readonly XamlRoot root;
    internal WindowCoreWindow(CoreWindow window, XamlRoot root)
    {
        this.window = window;
        this.root = root;
        CoreApplicationView = CoreApplication.Views.First(x => x.CoreWindow == window);
    }
    public override object PlatformWindow => window;
    public CoreApplicationView CoreApplicationView { get; }
    public CoreWindow CoreWindow => window;
    public override XamlRoot XamlRoot => root;
    public override UIElement RootContent
    {
        get => root.Content;
        set
        {
            var window = Windows.UI.Xaml.Window.Current;
            if (window.Content.XamlRoot == root)
            {
                window.Content = value;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
    public override double TitleBarLeftInset => CoreApplicationView.TitleBar.SystemOverlayLeftInset;
    public override double TitleBarRightInset => CoreApplicationView.TitleBar.SystemOverlayRightInset;
    public override Rect ClientBounds {
        get => window.Bounds;

        set => throw new NotSupportedException();
    }
    public override Rect Bounds
    {
        get
        {
            SelfNote.HasDisallowedPInvoke();
            var rect = WinWrapper.Windowing.Window.FromWindowHandle(WindowHandle).Bounds;
            return new(rect.X, rect.Y, rect.Width, rect.Height);
        }

        set
        {
            SelfNote.HasDisallowedPInvoke();
            Rectangle rect = new((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
            WinWrapper.Windowing.Window.FromWindowHandle(WindowHandle).Bounds = rect;
        }
    }
    public override bool ExtendsContentIntoTitleBar
    {
        get => CoreApplicationView.TitleBar.ExtendViewIntoTitleBar;
        set => CoreApplicationView.TitleBar.ExtendViewIntoTitleBar = value;
    }
    public override nint WindowHandle => window.As<ICoreWindowInterop>().WindowHandle;
    public override void Close()
    {
        if (CoreApplicationView.IsMain)
        {
            // if same calling thread
            if (CoreWindow == CoreWindow.GetForCurrentThread())
                _ = ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }
        else
        {
            window.Close();
        }
    }
    EventHandler? _Closed;
    DefferalCanceledEventHandler? _Closing;
    public override event EventHandler Closed
    {
        add
        {
            _Closed += value;
            if (_Closed is not null)
            {
                window.Closed -= Window_Closed;
                window.Closed += Window_Closed;
            }
        }
        remove
        {
            _Closed -= value;
            if (_Closed is null)
            {
                window.Closed -= Window_Closed;
            }
        }
    }

    private void Window_Closed(CoreWindow sender, CoreWindowEventArgs args)
    {
        _Closed?.Invoke(this, new());
    }

    public override event DefferalCanceledEventHandler Closing
    {
        add
        {
            _Closing += value;
            if (_Closing is not null)
            {
                var navManager = Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView();
                navManager.CloseRequested += WindowCoreWindow_CloseRequested;
                navManager.CloseRequested += WindowCoreWindow_CloseRequested;
            }

        }
        remove
        {
            _Closing -= value;
            if (_Closing is null)
            {
                var navManager = Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView();
                navManager.CloseRequested -= WindowCoreWindow_CloseRequested;
            }
        }
    }

    private async void WindowCoreWindow_CloseRequested(object sender, Windows.UI.Core.Preview.SystemNavigationCloseRequestedPreviewEventArgs e)
    {
        DefferalCanceledEventArgs args = new();
        var def = e.GetDeferral();
        _Closing.Invoke(this, args);
        await (args.DeferralInternal?.WaitAsync() ?? Task.CompletedTask);
        e.Handled = args.Cancel;
        def.Complete();
    }
    public override void Activate()
    {
        window.Activate();
    }
}