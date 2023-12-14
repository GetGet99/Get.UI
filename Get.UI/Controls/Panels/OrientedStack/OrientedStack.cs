namespace Get.UI.Controls.Panels;
[AttachedProperty(typeof(GridUnitType), "LengthType", GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(double), "LengthValue", GenerateLocalOnPropertyChangedMethod = true)]
[AttachedProperty(typeof(GridLength), "Length", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<Orientation>("Orientation", GenerateLocalOnPropertyChangedMethod = true, LocalOnPropertyChangedMethodWithParameter = false, LocalOnPropertyChangedMethodName = nameof(InvalidateArrange))]
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
    protected override Size MeasureOverride(Size availableSize)
        => 
        DoLogic(availableSize, false);
    protected override Size ArrangeOverride(Size finalSize)
        => DoLogic(finalSize, true);

    Size DoLogic(Size avaliableSize, bool shouldArrange)
    {
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
        var panelSize = SizeToOF(avaliableSize);

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
                pixelList.Add((length.Value, child));
            }
        }
        var remainingSpace = panelSize.Along - totalAbsolutePixel;
        if (remainingSpace < 0) remainingSpace = 0;
        List<(double pixel, UIElement ele)> measuredAutoList = new(count);
        foreach (var child in autoList)
        {
            var measuringSize = OFToSize((remainingSpace, panelSize.Opposite));
            child.Measure(measuringSize);
            var desiredSize = SizeToOF(child.DesiredSize);
            if (desiredSize.Along > remainingSpace) desiredSize.Along = remainingSpace;
            remainingSpace -= desiredSize.Along;
            measuredAutoList.Add((desiredSize.Along, child));
        }

        if (remainingSpace < 0) remainingSpace = 0;

        double lengthPerStar;
        if (totalStar is 0)
        {
            lengthPerStar = 1;
        } else
        {
            lengthPerStar = remainingSpace / totalStar;
            remainingSpace = 0;
        }
        var enumerators = new IEnumerator<(double pixel, UIElement ele)>[] {
            pixelList.GetEnumerator(),
            measuredAutoList.GetEnumerator(),
            starList.Select(x => (pixel: x.star * lengthPerStar, x.ele)).GetEnumerator()
        };
        // set all to first position
        foreach (var enu in enumerators) enu.MoveNext();

        double offset = 0;
        foreach (var child in Children)
        {
            var enu = enumerators.First(x => x.Current.ele == child);
            var size = OFToSize((enu.Current.pixel, panelSize.Opposite));
            child.Measure(size);
            var desiredSize = SizeToOF(child.DesiredSize);
            if (desiredSize.Opposite > maxOpposite) maxOpposite = desiredSize.Opposite;
            if (shouldArrange)
            {
                child.Arrange(
                    new(
                        OFToPoint((offset, 0)),
                        OFToSize((enu.Current.pixel, desiredSize.Opposite))
                    )
                );
            }
            offset += enu.Current.pixel;
            enu.MoveNext();
        }
        var along = panelSize.Along - remainingSpace;
        if (double.IsInfinity(along) || double.IsNaN(along)) along = offset;
        if (shouldArrange)
            return OFToSize((along, panelSize.Opposite));
        else
            return OFToSize((along, maxOpposite));
    }
}