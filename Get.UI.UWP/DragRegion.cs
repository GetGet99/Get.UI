﻿#nullable enable
using Get.UI.MotionDrag;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
namespace Get.UI.Controls;

[AttachedProperty(typeof(bool), "Clickable", typeof(FrameworkElement), Documentation = "/// This property does nothing. Maintain to keep the difference between UWP and WinUI 3 version the same")]
public partial class DragRegion : Grid
{
    public DragRegion()
    {
        var grid = new Grid() { Background = new SolidColorBrush(Colors.Transparent) };
        Canvas.SetZIndex(grid, int.MinValue);
        Children.Add(grid);
        Loaded += DragRegion_Loaded;
        Unloaded += DragRegion_Unloaded;
    }

    private void DragRegion_Unloaded(object sender, RoutedEventArgs e)
    {
        if (windowContext is CoreWindow)
            try
            {
                Window.Current.SetTitleBar(null);
            }
            catch { }
        else if (windowContext is AppWindow aw)
            try
            {
                aw.Frame.DragRegionVisuals.Remove(Children[0]);
            } catch { }
    }
    object? windowContext;
    private void DragRegion_Loaded(object sender, RoutedEventArgs e)
    {
        windowContext = GlobalContainerRect.GetWindowContext(XamlRoot);
        if (windowContext is CoreWindow)
            Window.Current.SetTitleBar(Children[0]);
        else if (windowContext is AppWindow aw)
            aw.Frame.DragRegionVisuals.Add(Children[0]);
    }

}