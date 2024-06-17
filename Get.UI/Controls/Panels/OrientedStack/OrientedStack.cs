using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace Get.UI.Controls.Panels;
[AttachedProperty(typeof(GridUnitType), "LengthType", GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(double), "LengthValue", GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(GridLength), "Length", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<Orientation>("Orientation", GenerateLocalOnPropertyChangedMethod = true)]
public partial class OrientedStack : Panel
{
    static partial void OnLengthChanged(DependencyObject obj, GridLength oldValue, GridLength newValue)
    {
        if (GetLengthValue(obj) != newValue.Value)
            SetLengthValue(obj, newValue.Value);
        if (GetLengthType(obj) != newValue.GridUnitType)
            SetLengthType(obj, newValue.GridUnitType);
        (VisualTreeHelper.GetParent(obj) as OrientedStack)?.InvalidateArrange();
    }
    static partial void OnLengthTypeChanged(DependencyObject obj, GridUnitType oldValue, GridUnitType newValue)
    {
        var length = GetLength(obj);
        length = new(length.Value, newValue);
        if (GetLength(obj) != length)
            SetLength(obj, length);
    }
    static partial void OnLengthValueChanged(DependencyObject obj, double oldValue, double newValue)
    {
        var length = GetLength(obj);
        length = new(newValue, length.GridUnitType);
        if (GetLength(obj) != length)
            SetLength(obj, length);
    }
    partial void OnOrientationChanged(Orientation oldValue, Orientation newValue)
    {
        InvalidateMeasure();
        InvalidateArrange();
    }
    protected override Size MeasureOverride(Size availableSize)
    {
#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        var orientation = Orientation;
        (double Along, double Opposite) SizeToOF(Size size) =>
            orientation is Orientation.Horizontal ?
            (size.Width, size.Height) : (size.Height, size.Width);
        Size OFToSize((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        var panelSize = SizeToOF(availableSize);
        var panelRemainingSize = panelSize;
        double totalUsed = 0;
        var count = Children.Count;
        List<(double pixel, UIElement ele)> pixelList = new(count);
        List<UIElement> autoList = new(count);
        List<(double star, UIElement ele)> starList = new(count);

        double totalAbsolutePixel = 0, totalStar = 0, maxOpposite = 0;
        foreach (var child in Children)
        {
            var length = GetLength(child);
            if (length.IsAuto)
            {
                autoList.Add(child);
            }
            else if (length.IsStar)
            {
                totalStar += length.Value;
                starList.Add((length.Value, child));
            }
            else
            {
                totalAbsolutePixel += length.Value;
            }
        }
        foreach (var (pixel, child) in pixelList)
        {
            child.Measure(OFToSize((pixel, panelSize.Opposite)));
            var (_, Opposite) = SizeToOF(child.DesiredSize);
            if (Opposite > maxOpposite)
                maxOpposite = Opposite;
        }
        panelRemainingSize.Along -= totalAbsolutePixel;
        totalUsed += totalAbsolutePixel;
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        foreach (var child in autoList)
        {
            child.Measure(OFToSize(panelRemainingSize));
            var (Along, Opposite) = SizeToOF(child.DesiredSize);
            if (Opposite > maxOpposite)
                maxOpposite = Opposite;
            totalUsed += Along;
            panelRemainingSize.Along -= Along;
            panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        }
        double maxStarSize = 0;
        foreach (var (star, child) in starList)
        {
            child.Measure(OFToSize(panelRemainingSize));
            var (Along, Opposite) = SizeToOF(child.DesiredSize);
            if (Opposite > maxOpposite)
                maxOpposite = Opposite;
            var starSize = Along / star;
            if (starSize > maxStarSize)
                maxStarSize = starSize;
        }
        var computed = maxStarSize * totalStar;
        panelRemainingSize.Along -= computed;
        totalUsed += computed;
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        var toReturn = OFToSize((totalUsed, Math.Min(maxOpposite, panelSize.Opposite)));
#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        return toReturn;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        var orientation = Orientation;
        (double Along, double Opposite) SizeToOF(Size size) =>
            orientation is Orientation.Horizontal ?
            (size.Width, size.Height) : (size.Height, size.Width);
        Size OFToSize((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        Point OFToPoint((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        var panelSize = SizeToOF(finalSize);
        var panelRemainingSize = panelSize;
        double totalAbsolutePixel = 0, totalStar = 0;
        foreach (var child in Children)
        {
            var length = GetLength(child);
            if (length.IsAuto)
            {
                var desiredSize = SizeToOF(child.DesiredSize);
                totalAbsolutePixel += desiredSize.Along;
            }
            else if (length.IsStar)
            {
                totalStar += length.Value;
            }
            else
            {
                totalAbsolutePixel += length.Value;
            }
        }
        panelRemainingSize.Along -= totalAbsolutePixel;
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        double alongOffset = 0;
        
        // To avoid divide by 0
        if (totalStar is 0) totalStar = 1;
        var starLength = panelRemainingSize.Along / totalStar;
        
        foreach (var child in Children)
        {
            var length = GetLength(child);
            if (length.IsAuto)
            {
                var desiredSize = SizeToOF(child.DesiredSize);
                child.Arrange(new(
                    OFToPoint((alongOffset, 0)),
                    OFToSize((desiredSize.Along, panelRemainingSize.Opposite))
                ));
                alongOffset += desiredSize.Along;
                panelRemainingSize.Along -= desiredSize.Along;
            }
            else if (length.IsStar)
            {
                var computedLength = starLength * length.Value;
                child.Arrange(new(
                    OFToPoint((alongOffset, 0)),
                    OFToSize((computedLength, panelRemainingSize.Opposite))
                ));
                alongOffset += computedLength;
                panelRemainingSize.Along -= computedLength;
            }
            else
            {
                child.Arrange(new(
                    OFToPoint((alongOffset, 0)),
                    OFToSize((length.Value, panelRemainingSize.Opposite))
                ));
                alongOffset += length.Value;
                panelRemainingSize.Along -= length.Value;
            }
        }
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);

#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        return OFToSize((panelSize.Along - panelRemainingSize.Along, Math.Min(panelRemainingSize.Opposite, panelSize.Opposite)));
    }
}