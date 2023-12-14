#if WINDOWS_UWP
using Windows.Internal;
using WinRT;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
#endif
namespace Get.UI.MotionDrag;

public readonly record struct GlobalContainerRect(Point WindowPosOffset, Rect ContainerRectToWindow, double RasterizationScale)
{
    public Rect ContainerRectToScreen => new(
        PointToScreen(default),
        ContainerRectToWindow.ToSize()
    );
    static Point GetPos(XamlRoot root)
    {
#if WINDOWS_UWP
        switch (root.UIContext.As<Windows.Internal.IUIContextPartner>().WindowContext)
        {
            case CoreWindow cw:
                return cw.Bounds.GetLocation();
            case AppWindow aw:
                var hwnd = aw.As<IApplicationWindow_HwndInterop>().WindowHandle;
                System.Drawing.Point pt = default;
                unsafe
                {
                    _ = Windows.Win32.PInvoke.MapWindowPoints(new((nint)hwnd.Value), default, &pt, 1);
                }
                return new(pt.X, pt.Y);
        }
        return default;
#else
        var hwnd = root.ContentIslandEnvironment.AppWindowId;
        System.Drawing.Point pt = default;
        unsafe
        {
            _ = Windows.Win32.PInvoke.MapWindowPoints(new((nint)hwnd.Value), default, &pt, 1);
        }
        return new(pt.X, pt.Y);
#endif
    }
    internal static nint GetHwnd(XamlRoot root)
    {
#if WINDOWS_UWP
        switch (root.UIContext.As<Windows.Internal.IUIContextPartner>().WindowContext)
        {
            case CoreWindow cw:
                return cw.As<ICoreWindowInterop>().WindowHandle;
            case AppWindow aw:
                return (nint)aw.As<IApplicationWindow_HwndInterop>().WindowHandle.Value;
        }
        return default;
#else
        var hwnd = root.ContentIslandEnvironment.AppWindowId;
        return (nint)hwnd.Value;
#endif
    }
    public static GlobalContainerRect GetFromXamlRoot(XamlRoot root, Rect ContainerRect)
        => new(GetPos(root), ContainerRect, root.RasterizationScale);
    public static GlobalContainerRect GetFromContainer(UIElement container)
    {
        var margin = ((FrameworkElement)container.XamlRoot.Content).Margin;
        var pos = container.TransformToVisual(container.XamlRoot.Content).TransformPoint(
            new(margin.Left, margin.Top)
        );
        var size = container.ActualSize;
        return GetFromXamlRoot(container.XamlRoot, new(pos, new Size(size.X, size.Y)));
    }
    public Point PointToScreen(Point pt)
    {
        return new(
            (pt.X + ContainerRectToWindow.X) * RasterizationScale + WindowPosOffset.X,
            (pt.Y + ContainerRectToWindow.Y) * RasterizationScale + WindowPosOffset.Y
        );
    }
    public Point PointToClient(Point pt)
    {
        return new(
            (pt.X - WindowPosOffset.X) / RasterizationScale - ContainerRectToWindow.X,
            (pt.Y - WindowPosOffset.Y) / RasterizationScale - ContainerRectToWindow.Y
        );
    }
    public Point ScaleToNewContainer(GlobalContainerRect newContainer, Point pt)
        => new(
            pt.X * RasterizationScale / newContainer.RasterizationScale,
            pt.Y * RasterizationScale / newContainer.RasterizationScale
        );
    public Point ToNewContainer(GlobalContainerRect newContainer, Point pt)
        => newContainer.PointToClient(PointToScreen(pt));
    public Rect ToNewContainer(GlobalContainerRect newContainer, Rect rect)
        => new(
            newContainer.PointToClient(PointToScreen(rect.GetLocation())),
            newContainer.PointToClient(PointToScreen(new(rect.Right, rect.Bottom)))
        );
}