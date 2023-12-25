using Get.UI.MotionDrag;

namespace Get.UI.Controls.Containers;

[DependencyProperty(typeof(Orientation), "ReorderOrientation", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public partial class MotionDragContainer : ItemsControl
{
    public MotionDragContainer()
    {
        DefaultStyleKey = typeof(ItemsControl);
        //Background = new SolidColorBrush(Colors.Transparent);
        AllowDrop = true;
        AnimationController = new(this);
        ConnectionContext = new();
        Loaded += MotionDragContainer_Loaded;
        Unloaded += MotionDragContainer_Unloaded;
    }

    

    //int curItemIndex;
    //int curHoverItemIndex;
    //double tranOffset;
    internal UIElement? SafeContainerFromIndex(int idx) => idx < 0 ? null : ContainerFromIndex(idx) as UIElement;
    internal int SafeIndexFromContainer(DependencyObject? obj) => obj is null ? -1 : IndexFromContainer(obj);
    //bool DidSetZIndex = false;
    MotionDragItem CurrentManipulationItem;
    partial void OnConnectionContextChanged(MotionDragConnectionContext? oldValue, MotionDragConnectionContext? newValue)
    {
        oldValue?.Remove(this);
        if (IsLoaded)
            newValue?.Add(this);
    }
    internal int ItemsCount
    {
        get
        {
            if (ItemsSource is IList itemsSource)
                return itemsSource.Count;
            return Items.Count;
        }
    }
    readonly MotionDragReorderContainerController AnimationController;
    partial void OnReorderOrientationChanged(Orientation oldValue, Orientation newValue)
    {
        int i = 0;
        while (SafeContainerFromIndex(i++)?.FindDescendantOrSelf<MotionDragItem>() is { } st)
        {
            st.OnReorderOrientationUpdated();
        }
    }
    internal object? ObjectFromSRI(MotionDragItem item)
    {
        return FirstNotNullOrNull(item.FindAscendants(), x => ItemFromContainer(x));

    }
    TReturn? FirstNotNullOrNull<TReturn, TSource>(IEnumerable<TSource> t, Func<TSource, TReturn?> func) where TReturn : class
    {
        foreach (var item in t)
        {
            if (func(item) is TReturn output) return output;
        }
        return null;
    }
}
partial class MotionDragReorderContainerController(MotionDragContainer self)
{
    [Property(OnChanged = nameof(UpdateAnimated))]
    int startRemoveIndex;
    [Property(OnChanged = nameof(UpdateAnimated))]
    int startShiftIndex;
    [Property(OnChanged = nameof(UpdateAnimated))]
    double shiftAmount;
    double[]? positions;
    double[]? positionsremoved;
    public Point PositionOfItemAtIndex(int index)
    {
        bool afterLast = false;
        if (index >= self.ItemsCount)
        {
            afterLast = true;
            index = self.ItemsCount - 1;
        }
        if (self.SafeContainerFromIndex(index) is not { } curItem || curItem.FindDescendantOrSelf<MotionDragItem>() is not { } st)
            throw new InvalidOperationException();
        if (positions is null) UpdateAnimated();
        var position = curItem.TransformToVisual(self.ItemPlace).TransformPoint(default);
        if (afterLast)
        {
            return self.ReorderOrientation is Orientation.Horizontal ?
                new(position.X + curItem.ActualSize.X, positions[index]) :
                new(positions[index], position.Y + curItem.ActualSize.Y);
        }
        else
        {
            return self.ReorderOrientation is Orientation.Horizontal ?
                new(positions[index], position.Y) :
                new(position.X, positions[index]);
        }
    }
    [MemberNotNull(nameof(positions))]
    [MemberNotNull(nameof(positionsremoved))]
    void UpdateAnimated()
    {
        var orientation = self.ReorderOrientation;

        Point PointAt(double tran) => orientation is Orientation.Horizontal ? new(tran, 0) : new(0, tran);
        double TranAt(Point pt) => orientation is Orientation.Horizontal ? pt.X : pt.Y;

        var shift = PointAt(shiftAmount);
        var itemCount = self.ItemsCount;
        if (positions is null || positions.Length != itemCount + 1)
            positions = new double[itemCount + 1];
        if (positionsremoved is null || positionsremoved.Length != itemCount + 1)
            positionsremoved = new double[itemCount + 1];
        for (int i = 0; i < itemCount; i++)
        {
            // do not translate the one we are showing on popup
            if (i == startRemoveIndex)
            {
                continue;
            }
            if (self.SafeContainerFromIndex(i) is not { } curItem || curItem.FindDescendantOrSelf<MotionDragItem>() is not { } st)
                continue;

            var position = curItem.TransformToVisual(self.ItemPlace).TransformPoint(default);


            Point translationAmount = default;
            if (i > 0 && i > startRemoveIndex)
                if (self.SafeContainerFromIndex(i - 1) is { } prevItem)
                {
                    var d = curItem.TransformToVisual(prevItem).TransformPoint(default);
                    translationAmount = translationAmount.Subtract(d);
                }
            positionsremoved[i] = TranAt(position) + TranAt(translationAmount);
            if (i >= startShiftIndex)
                translationAmount = translationAmount.Add(shift);
            positions[i] = TranAt(position) + TranAt(translationAmount);
            st.TemporaryAnimateTranslation(translationAmount.X, translationAmount.Y);
            if (i == itemCount - 1)
            {
                positions[i + 1] = positions[i] +
                    (orientation is Orientation.Horizontal ? curItem.ActualSize.X : curItem.ActualSize.Y);
                positionsremoved[i + 1] = positionsremoved[i] +
                    (orientation is Orientation.Horizontal ? curItem.ActualSize.X : curItem.ActualSize.Y);
            }
            if (i == startRemoveIndex + 1)
            {
                positionsremoved[i - 1] = positionsremoved[i];
                positions[i - 1] = positions[i];
            }
            else if (i == startRemoveIndex + 2)
            {
                positionsremoved[i - 1] = positionsremoved[i];
                positions[i - 1] = positions[i];
            }

        }
    }
    public void Reset()
    {
        startRemoveIndex = startShiftIndex = self.ItemsCount;
        shiftAmount = 0;
        positions = positionsremoved = null;
        int i = 0;
        while (self.SafeContainerFromIndex(i++)?.FindDescendantOrSelf<MotionDragItem>() is { } st2)
        {
            st2.ResetTranslationImmedietly();
        }
    }
    public int IndexOfItemAt(double posX, double posY)
    {
        //if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift) is Windows.UI.Core.CoreVirtualKeyStates.Down)
        //{
        //    Debugger.Break();
        //}
        if (positionsremoved is null) UpdateAnimated();
        var pos = self.ReorderOrientation is Orientation.Vertical ? posY : posX;
        var idx = Array.BinarySearch(positionsremoved, pos);
        if (idx < 0)
        {
            idx = ~idx;
        }
        //
        if (idx >= 1 && idx < positionsremoved.Length)
        {
            if (pos <= (positionsremoved[idx - 1] + positionsremoved[idx]) / 2)
            {
                idx--;
                if (idx == startRemoveIndex + 2)
                    idx--;
            }
            else if (idx == startRemoveIndex + 3) idx--;
        }
        if (idx >= positionsremoved.Length) idx = positionsremoved.Length;
        return idx;
    }
}