using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Internal;
using Windows.UI.WindowManagement;
using WinRT;

namespace Get.UI.Windowing;

public class WindowAppWindow : Window
{
    readonly AppWindow window;
    readonly XamlRoot root;
    internal WindowAppWindow(AppWindow window, XamlRoot root)
    {
        this.window = window;
        this.root = root;
    }

    private async void Window_CloseRequested(AppWindow sender, AppWindowCloseRequestedEventArgs e)
    {
        DefferalCanceledEventArgs args = new();
        var def = e.GetDeferral();
        _Closing?.Invoke(this, args);
        await (args.DeferralInternal?.WaitAsync() ?? Task.CompletedTask);
        e.Cancel = args.Cancel;
        def.Complete();
    }

    private void Window_Closed(AppWindow sender, AppWindowClosedEventArgs args)
    {
        _Closed?.Invoke(this, new());
    }

    public override object PlatformWindow => window;
    public AppWindow AppWindow => window;
    public override XamlRoot XamlRoot => root;
    public override UIElement RootContent
    {
        get => ElementCompositionPreview.GetAppWindowContent(window);
        set => ElementCompositionPreview.SetAppWindowContent(window, value);
    }
    public override double TitleBarLeftInset => CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset;
    public override double TitleBarRightInset => CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset;
    public override Rect Bounds {
        get
        {
            var placement = window.GetPlacement();
            return new Rect(placement.Offset, placement.Size);
        }

        set
        {
            window.RequestSize(value.ToSize());
            var currentWindowBounds = WindowCoreWindow.CurrentThreadCoreWindow.Bounds;
            window.RequestMoveRelativeToCurrentViewContent(
                new(value.X - currentWindowBounds.X, value.Y - currentWindowBounds.Y)
            );
        }
    }
    public override Rect ClientBounds { get => SelfNote.ThrowNotImplemented<Rect>(); set => SelfNote.ThrowNotImplemented<Rect>(); }
    public override nint WindowHandle => (nint)window.As<IApplicationWindow_HwndInterop>().WindowHandle.Value;
    public override bool ExtendsContentIntoTitleBar {
        get => AppWindow.TitleBar.ExtendsContentIntoTitleBar;
        set => AppWindow.TitleBar.ExtendsContentIntoTitleBar = value;
    }
    public override void Close() => _ = window.CloseAsync();
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
    public override event DefferalCanceledEventHandler Closing
    {
        add
        {
            _Closing += value;
            if (_Closing is not null)
            {
                window.CloseRequested -= Window_CloseRequested;
                window.CloseRequested += Window_CloseRequested;
            }
        }
        remove
        {
            _Closing -= value;
            if (_Closing is null)
            {
                window.CloseRequested -= Window_CloseRequested;
            }
        }
    }
    public override void Activate()
    {
        _ = window.TryShowAsync();
    }
}