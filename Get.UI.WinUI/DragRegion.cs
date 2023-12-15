using Get.UI.MotionDrag;
using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Win32;
using WinWrapper.Input;
using WinWrapper.Windowing;
using DispatcherQueueTimer = Microsoft.UI.Dispatching.DispatcherQueueTimer;
using Window = WinWrapper.Windowing.Window;

namespace Get.UI.Controls;

public class DragRegion : Control
{
    Border RootElement => (Border)GetTemplateChild(nameof(RootElement));
    public DragRegion()
    {
        DefaultStyleKey = typeof(DragRegion);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RootElement.SizeChanged += RootElement_SizeChanged;
        RootElement.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        RootElement.ManipulationStarting += RootElement_ManipulationStarting;
        RootElement.ManipulationStarted += RootElement_ManipulationStarted;
        RootElement.ManipulationDelta += RootElement_ManipulationDelta;
        RootElement.ManipulationCompleted += RootElement_ManipulationCompleted;
        //RootElement.PointerPressed += RootElement_PointerPressed;
        //timer = DispatcherQueue.CreateTimer();
        //timer.Interval = TimeSpan.FromMilliseconds(200);
        //timer.Tick += (_, _) => TimerDone();
        //Loaded += DragRegion_Loaded;
        //Unloaded += DragRegion_Unloaded;
    }

    private void RootElement_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
        var window = Window.GetWindowFromPoint(Cursor.Position);
        var window2 = Window.FromWindowHandle((nint)XamlRoot.ContentIslandEnvironment.AppWindowId.Value);
        
        PInvoke.SetCapture(new(window2.Handle));
        //window2.Focus();
        //window2.SendMessage(WindowMessages.LButtonDown, 0, 0);
        //window2.SendMessage(WindowMessages.LButtonUp, 0, 0);
        window2.SendMessage(WindowMessages.SysCommand, (nuint)0xF012, 0);
    }

    private void DragRegion_Unloaded(object sender, RoutedEventArgs e)
    {
        incps = null;
    }

    private void DragRegion_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void Incps_PointerEntered(InputNonClientPointerSource sender, NonClientPointerEventArgs args)
    {
        
    }

    private void Incps_PointerPressed(InputNonClientPointerSource sender, NonClientPointerEventArgs args)
    {
        
    }

    InputNonClientPointerSource? incps;
    DispatcherQueueTimer timer;

    private void RootElement_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        //timer.Start();
    }
    void TimerDone()
    {
        if (!WinWrapper.Input.Cursor.IsLeftButtonDown)
        {
            timer.Start();
            return;
        }
        if (incps is null)
        {
            appWindow = AppWindow.GetFromWindowId(XamlRoot.ContentIslandEnvironment.AppWindowId);
            incps = InputNonClientPointerSource.GetForWindowId(appWindow.Id);
            incps.PointerPressed += Incps_PointerPressed;
            incps.PointerEntered += Incps_PointerEntered;
        }
        if (incps is null) return;
        try
        {
            var rect = GlobalContainerRect.GetFromContainer(this).ContainerRectToWindow;
            if (rect.Y > 0)
            {
                rect = rect with { Y = 0, Height = rect.Bottom - 0 };
            }
            incps.ClearRegionRects(NonClientRegionKind.Caption);
            incps.SetRegionRects(NonClientRegionKind.Caption, new RectInt32[]
            {
                new((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height)
            });
        }
        catch { }
    }

    private void RootElement_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        e.Handled = true;
        //InputNonClientPointerSource.GetForWindowId(appWindow!.Id)
        //.ClearRegionRects(NonClientRegionKind.Caption);
    }

    AppWindow? appWindow;
    Point translationDeltaRemaining;
    private void RootElement_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        e.Handled = true;
        translationDeltaRemaining = translationDeltaRemaining.Add(e.Delta.Translation.Multiply(XamlRoot.RasterizationScale));
        var moveAmount = new PointInt32((int)translationDeltaRemaining.X, (int)translationDeltaRemaining.Y);
        var newPos = appWindow!.Position;
        newPos.X += moveAmount.X;
        newPos.Y += moveAmount.Y;
        appWindow!.Move(newPos);
        translationDeltaRemaining = new(translationDeltaRemaining.X - moveAmount.X, translationDeltaRemaining.Y - moveAmount.Y);
    }

    private void RootElement_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        e.Handled = true;
        appWindow = AppWindow.GetFromWindowId(XamlRoot.ContentIslandEnvironment.AppWindowId);
        translationDeltaRemaining = e.Cumulative.Translation.Multiply(XamlRoot.RasterizationScale);
        var moveAmount = new PointInt32((int)translationDeltaRemaining.X, (int)translationDeltaRemaining.Y);
        var newPos = appWindow.Position;
        newPos.X += moveAmount.X;
        newPos.Y += moveAmount.Y;
        appWindow.Move(newPos);
        translationDeltaRemaining = new(translationDeltaRemaining.X - moveAmount.X, translationDeltaRemaining.Y - moveAmount.Y);
    }

    private void RootElement_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
    {
        e.Handled = true;
    }

}
