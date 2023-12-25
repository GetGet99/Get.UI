using Windows.Graphics;
using System.Drawing;
namespace Get.UI.Controls;

public partial class DragRegion : Grid
{
    public DragRegion()
    {
        SizeChanged += DragRegion_SizeChanged;
        Loaded += DragRegion_Loaded;
        Unloaded += DragRegion_Unloaded;
        PointerMoved += DragRegion_PointerMoved;
    }

    private void DragRegion_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        UpdateRegion();
        e.Handled = true;
    }

    private void DragRegion_Unloaded(object sender, RoutedEventArgs e)
    {
        current?.ClearAllRegionRects();
    }

    private void DragRegion_Loaded(object sender, RoutedEventArgs e)
    {
        current?.ClearAllRegionRects();
        current = InputNonClientPointerSource.GetForWindowId(XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    private void DragRegion_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateRegion();
    }

    InputNonClientPointerSource? current;
    void UpdateRegion()
    {
        current?.SetRegionRects(NonClientRegionKind.Caption,
            InvertRegion(GetClickableRectangles(this, this), (int)ActualWidth, (int)ActualHeight)
        );
    }
    static IEnumerable<Rectangle> GetClickableRectangles(UIElement element, UIElement relativeTo)
    {
        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child switch
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
    // This code is written by ChatGPT with some modification
    static RectInt32[] InvertRegion(IEnumerable<Rectangle> uiPlaces, int width, int height)
    {
        // Add the initial full area rectangle
        List<Rectangle> invertedRegion = [new(0, 0, width, height)];

        // Remove each UI rectangle from the full area to get the inverted region
        foreach (Rectangle uiRect in uiPlaces)
        {
            List<Rectangle> newInvertedRegion = [];

            foreach (Rectangle regionRect in invertedRegion)
            {
                if (regionRect.IntersectsWith(uiRect))
                {
                    // Split the region into parts and keep the parts that are outside the UI rectangle
                    IEnumerable<Rectangle> splitRectangles = SplitRectangle(regionRect, uiRect);

                    foreach (Rectangle splitRect in splitRectangles)
                    {
                        if (splitRect.Width > 0 && splitRect.Height > 0)
                        {
                            newInvertedRegion.Add(splitRect);
                        }
                    }
                }
                else
                {
                    newInvertedRegion.Add(regionRect);
                }
            }

            invertedRegion = newInvertedRegion;
        }

        return invertedRegion.Select(x => new RectInt32(x.X, x.Y, x.Width, x.Height)).ToArray();
    }
    // This code is written by ChatGPT with some modification
    static IEnumerable<Rectangle> SplitRectangle(Rectangle originalRect, Rectangle splitRect)
    {
        // Split the original rectangle by the split rectangle

        if (originalRect.IntersectsWith(splitRect))
        {
            Rectangle intersection = Rectangle.Intersect(originalRect, splitRect);

            if (intersection.Width > 0 && intersection.Height > 0)
            {
                // Split the original rectangle into parts excluding the split rectangle

                // Top part
                yield return new Rectangle(originalRect.X, originalRect.Y, originalRect.Width, intersection.Top - originalRect.Top);

                // Bottom part
                yield return new Rectangle(originalRect.X, intersection.Bottom, originalRect.Width, originalRect.Bottom - intersection.Bottom);

                // Left part
                yield return new Rectangle(originalRect.X, intersection.Top, intersection.Left - originalRect.Left, intersection.Height);

                // Right part
                yield return new Rectangle(intersection.Right, intersection.Top, originalRect.Right - intersection.Right, intersection.Height);

                yield break;
            }
        }

        yield return originalRect;
    }
}