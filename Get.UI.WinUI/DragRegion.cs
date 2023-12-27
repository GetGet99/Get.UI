using Windows.Graphics;
using System.Drawing;
namespace Get.UI.Controls;
[AttachedProperty(typeof(bool), "Clickable", typeof(FrameworkElement), GenerateLocalOnPropertyChangedMethod = true)]
public partial class DragRegion : Grid
{
    public DragRegion()
    {
        SizeChanged += DragRegion_SizeChanged;
        Loaded += DragRegion_Loaded;
        Unloaded += DragRegion_Unloaded;
    }

    private void XamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        UpdateRegion();
    }
    readonly static Dictionary<object, DragRegion> mapping = [];
    static partial void OnClickableChanged(FrameworkElement obj, bool oldValue, bool newValue)
    {
        if (newValue)
        {
            obj.SizeChanged -= Obj_SizeChanged;
            obj.Loaded -= Obj_Loaded;
            obj.Unloaded -= Obj_Unloaded;
            obj.SizeChanged += Obj_SizeChanged;
            obj.Loaded += Obj_Loaded;
            obj.Unloaded += Obj_Unloaded;
            Update(obj);
        } else if (oldValue)
        {
            obj.SizeChanged -= Obj_SizeChanged;
            obj.Loaded -= Obj_Loaded;
            obj.Unloaded -= Obj_Unloaded;
            Update(obj);
            mapping.Remove(obj);
        }
    }

    private static void Obj_Unloaded(object sender, RoutedEventArgs e) => Update(sender);

    private static void Obj_Loaded(object sender, RoutedEventArgs e) => Update(sender);
    static void Obj_SizeChanged(object sender, SizeChangedEventArgs e) => Update(sender);
    static void Update(object sender)
    {
        var dragRegion = (sender as DependencyObject)?.FindAscendant<DragRegion>();
        if (dragRegion is null)
        {
            if (!mapping.TryGetValue(sender, out dragRegion))
                return;
        }
        else
            mapping[sender] = dragRegion;
        dragRegion?.UpdateRegion();
    }

    private void DragRegion_Unloaded(object sender, RoutedEventArgs e)
    {
        XamlRoot.Changed -= XamlRoot_Changed;
        try
        {
            current?.ClearAllRegionRects();
        } catch
        {
            current = null;
        }
    }

    private void DragRegion_Loaded(object sender, RoutedEventArgs e)
    {
        XamlRoot.Changed -= XamlRoot_Changed;
        XamlRoot.Changed += XamlRoot_Changed;
        current?.ClearAllRegionRects();
        current = InputNonClientPointerSource.GetForWindowId(XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    private void DragRegion_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateRegion();
    }

    InputNonClientPointerSource? current;
    public void UpdateRegion()
    {
        try
        {
            current?.SetRegionRects(NonClientRegionKind.Caption,
            [new(0, 0, (int)ActualWidth, (int)ActualHeight)]
        );
            current?.SetRegionRects(NonClientRegionKind.Passthrough,
                (from x in GetClickableRectangles(this, this) select new RectInt32(x.X, x.Y, x.Width, x.Height)).ToArray()
            );
        } catch
        {
            current = null;
        }
    }
    static IEnumerable<Rectangle> GetClickableRectangles(UIElement element, UIElement relativeTo)
    {
        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if ((child is FrameworkElement fe && GetClickable(fe)) ||
            child switch
            {
                Panel panel => panel.Background == null,
                Control control => control.Background == null,
                Border border => border.Background == null,
                ContentPresenter contentPresenter => contentPresenter.Background == null,
                _ => false
            })
                // this element is probably designed to be click-through
                foreach (var rect in GetClickableRectangles((UIElement)child, relativeTo))
                    yield return rect;
            else
                // this element is probably not designed to be click through
                yield return GetRect((UIElement)child, relativeTo);
        }
    }
    static Rectangle GetRect(UIElement element, UIElement relativeTo)
    {
        var pt = element.TransformToVisual(relativeTo).TransformPoint(default);
        return new() { X = (int)pt.X, Y = (int)pt.Y, Width = (int)element.ActualSize.X, Height = (int)element.ActualSize.Y };
    }
}