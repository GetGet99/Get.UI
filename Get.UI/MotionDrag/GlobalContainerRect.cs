#if WINDOWS_UWP
using Windows.Internal;
using WinRT;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
#endif
namespace Get.UI.MotionDrag;

public readonly record struct GlobalContainerRect(Point WindowPosOffset, Rect ContainerRectToWindow, double RasterizationScale, bool IsValid = true)
{
    public static GlobalContainerRect Invalid => new(default, default, default, IsValid: false);
    public Rect ContainerRectToScreen => new(
        PointToScreen(default),
        ContainerRectToWindow.ToSize()
    );
    internal static Point GetPos(nint hwnd)
    {
        System.Drawing.Point pt = default;
        unsafe
        {
            _ = Windows.Win32.PInvoke.MapWindowPoints(new(hwnd), default, &pt, 1);
        }
        return new(pt.X, pt.Y);
    }
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
#if WINDOWS_UWP
    internal static object GetWindowContext(XamlRoot root)
    {
        return root.UIContext.As<Windows.Internal.IUIContextPartner>().WindowContext;
    }
#endif
    public static GlobalContainerRect GetFromXamlRoot(XamlRoot? root, Rect ContainerRect)
    {
        if (root is null) return Invalid;
        return new(GetPos(root), ContainerRect, root.RasterizationScale);
    }
    public static GlobalContainerRect GetFromContainer(UIElement container)
    {
        var rootElement = container.XamlRoot?.Content as FrameworkElement;
        if (rootElement is null) return Invalid;
        var margin = rootElement.Margin;
        var pos = container.TransformToVisual(rootElement).TransformPoint(
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