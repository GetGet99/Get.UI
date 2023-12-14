using Get.UI.MotionDrag;

namespace Get.UI.Controls.Containers;

[DependencyProperty(typeof(Orientation), "ReorderOrientation", GenerateLocalOnPropertyChangedMethod = true)]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public partial class MotionDragContainer : ItemsControl
{
    public MotionDragContainer()
    {
        DefaultStyleKey = typeof(ItemsControl);
        Background = new SolidColorBrush(Colors.Transparent);
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
    //internal void MotionDragItemManipulationStart(MotionDragItem item)
    //{
    //    IsReadyForNextReorderManipulation = false;
    //    CurrentManipulationItem = item;
    //    var idx = IndexFromContainer(item);
    //    if (idx is -1) idx = IndexFromContainer(item.FindAscendant<ContentPresenter>());
    //    if (idx is -1) Debugger.Break();
    //    curItemIndex = curHoverItemIndex = idx;
    //    if (ContainerFromIndex(idx) is UIElement ele
    //        && Canvas.GetZIndex(ele) is 0)
    //    {
    //        Canvas.SetZIndex(ele, 1);
    //        DidSetZIndex = true;
    //    }
    //    else DidSetZIndex = false;
    //    tranOffset = 0;
    ////}
    //internal void MotionDragItemManipulationDelta(MotionDragItem item, double tranX, double tranY)
    //{
    //    var orientation = ReorderOrientation;
    //    double tran = orientation is Orientation.Horizontal ? tranX : tranY;
    //    tranOffset += tran;

    //    Point PointAt(double tran) => orientation is Orientation.Horizontal ? new(tran, 0) : new(0, tran);

    //    double TranAtPt(Point pt) => orientation is Orientation.Horizontal ? pt.X : pt.Y;
    //    double TranAtVec2(Vector2 vec) => orientation is Orientation.Horizontal ? vec.X : vec.Y;
    //    double TranAtSize(Size size) => orientation is Orientation.Horizontal ? size.Width : size.Height;

    //    // move backward
    //    if (curHoverItemIndex <= curItemIndex)
    //    {
    //        // while previous item exists
    //        while (SafeContainerFromIndex(curHoverItemIndex - 1) is { } prevItem)
    //        {
    //            var prevTest = TranAtPt(prevItem.TransformToVisual(item).TransformPoint(PointAt(-tranOffset))) + TranAtVec2(prevItem.ActualSize) / 2;

    //            if (prevTest > 0)
    //            {
    //                curHoverItemIndex--;
    //                // move to previous item
    //                if (prevItem.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //                {
    //                    // animate
    //                    if (SafeContainerFromIndex(curHoverItemIndex + 1) is { } curItem)
    //                    {
    //                        var d = curItem.TransformToVisual(prevItem).TransformPoint(default);
    //                        st.TemporaryAnimateTranslation(d.X, d.Y);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    } else
    //    {
    //        while (SafeContainerFromIndex(curHoverItemIndex) is { } curItem)
    //        {
    //            var prevTest = TranAtPt(curItem.TransformToVisual(item).TransformPoint(PointAt(-tranOffset))) - TranAtVec2(curItem.ActualSize) / 2;
    //            if (prevTest > 0)
    //            {
    //                curHoverItemIndex--;
    //                // move to previous item
    //                if (curItem.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //                {
    //                    // animate
    //                    st.TemporaryAnimateTranslation(0, 0);
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    // move forward
    //    if (curHoverItemIndex >= curItemIndex)
    //    {
    //        while (SafeContainerFromIndex(curHoverItemIndex + 1) is { } nextItem)
    //        {
    //            var nextTest = TranAtPt(item.TransformToVisual(nextItem).TransformPoint(PointAt(tranOffset))) + TranAtVec2(nextItem.ActualSize) / 2;

    //            if (nextTest > 0)
    //            {
    //                curHoverItemIndex++;
    //                // move to next item
    //                if (nextItem.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //                {
    //                    // animate
    //                    if (SafeContainerFromIndex(curHoverItemIndex - 1) is { } curItem)
    //                    {
    //                        var d = curItem.TransformToVisual(nextItem).TransformPoint(default);
    //                        st.TemporaryAnimateTranslation(d.X, d.Y);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    } else
    //    {
    //        while (SafeContainerFromIndex(curHoverItemIndex) is { } curItem)
    //        {
    //            var nextTest = TranAtPt(item.TransformToVisual(curItem).TransformPoint(PointAt(tranOffset))) - TranAtVec2(curItem.ActualSize) / 2;

    //            if (nextTest > 0)
    //            {
    //                curHoverItemIndex++;
    //                // move to next item
    //                if (curItem.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //                {
    //                    st.TemporaryAnimateTranslation(0, 0);
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //}
    //internal async void MotionDragItemManipulationEnd(MotionDragItem item)
    //{
    //    if (DidSetZIndex &&
    //        ContainerFromIndex(curItemIndex) is UIElement ele)
    //        Canvas.SetZIndex(ele, 0);
    //    if (curHoverItemIndex != curItemIndex)
    //    {
    //        if ((SafeContainerFromIndex(curHoverItemIndex) is { } curItem))
    //        {
    //            var d = curItem.TransformToVisual(item).TransformPoint(default);
    //            await item.TemporaryAnimateTranslationAsync(d.X, d.Y);
    //            int i = 0;
    //            while (SafeContainerFromIndex(i++)?.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //            {
    //                st.ResetTranslationImmedietly();
    //            }
    //            var itemSource = ItemsSource;
    //            if (itemSource is null)
    //            {
    //                var itemToMove = Items[curItemIndex];
    //                Items.RemoveAt(curItemIndex);
    //                Items.Insert(curHoverItemIndex, itemToMove);
    //            }
    //            else if (itemSource is IList list)
    //            {
    //                var itemToMove = list[curItemIndex];
    //                list.RemoveAt(curItemIndex);
    //                list.Insert(curHoverItemIndex, itemToMove);
    //                if (list is not INotifyCollectionChanged)
    //                {
    //                    // refresh ItemSource
    //                    ItemsSource = null;
    //                    ItemsSource = list;
    //                }
    //            } else
    //            {
    //                IsReadyForNextReorderManipulation = true;
    //                throw new NotSupportedException("ItemSource must implement IList");
    //            }
    //            IsReadyForNextReorderManipulation = true;
    //        }
    //    } else
    //    {
    //        await item.TemporaryAnimateTranslationAsync(0, 0);
    //        IsReadyForNextReorderManipulation = true;
    //    }
    //}
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
    //Point pos;
    //protected override void OnDragEnter(DragEventArgs e)
    //{
    //    int i = 0;
    //    while (SafeContainerFromIndex(i++) is { } container && container.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //    {
    //        if (st != CurrentManipulationItem)
    //        {
    //            container.IsHitTestVisible = false;
    //        }
    //    }
    //    pos = e.GetPosition(this);
    //    CurrentManipulationItem.Opacity = 1;
    //    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
    //    e.Handled = true;
    //}
    //internal void MotionDragItemDragEnter(DragEventArgs e)
    //{

    //}
    //protected override void OnDragOver(DragEventArgs e)
    //{
    //    var pos2 = e.GetPosition(this);
    //    e.DragUIOverride.IsContentVisible = false;
    //    e.DragUIOverride.IsGlyphVisible = false;
    //    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
    //    MotionDragItemManipulationDelta(
    //        CurrentManipulationItem,
    //        pos2.X - pos.X,
    //        pos2.Y - pos.Y
    //    );
    //    pos = pos2;

    //    e.Handled = true;
    //}
    //protected override void OnDragLeave(DragEventArgs e)
    //{
    //    int i = 0;
    //    while (SafeContainerFromIndex(i++) is { } container && container.FindDescendantOrSelf<MotionDragItem>() is { } st)
    //    {
    //        if (st != CurrentManipulationItem)
    //        {
    //            container.IsHitTestVisible = true;
    //        }
    //    }
    //    CurrentManipulationItem.Opacity = 0;
    //    e.Handled = true;
    //    //this.FindDescendant<OrientedStackForContainer>().IsHitTestVisible = true;
    //}
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