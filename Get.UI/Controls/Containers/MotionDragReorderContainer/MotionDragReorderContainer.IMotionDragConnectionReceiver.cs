using Get.UI.MotionDrag;

namespace Get.UI.Controls.Containers;

partial class MotionDragContainer : IMotionDragConnectionReceiver
{
    bool IMotionDragConnectionReceiver.IsVisibleAt(Point pt)
    {
        if (!(XamlRoot.IsHostVisible && Visibility is Visibility.Visible))
            return false;
        var ptScreen = GlobalRectangle.WindowPosOffset.Add(pt);
        return WinWrapper.Windowing.Window.FromLocation(
            (int)ptScreen.X,
            (int)ptScreen.Y
        ).Root == 
        WinWrapper.Windowing.Window.FromWindowHandle(GlobalContainerRect.GetHwnd(XamlRoot)).Root;
    }
    bool useCached = false; // warning: doesn't work, fix before turning this back to true
    GlobalContainerRect _globalRectangle;
    GlobalContainerRect GlobalRectangle => useCached ? _globalRectangle : GlobalContainerRect.GetFromContainer(this);
    GlobalContainerRect IMotionDragConnectionReceiver.GlobalRectangle => GlobalRectangle;
    void IMotionDragConnectionReceiver.DragEnter(object? sender, object? item, int senderIndex, DragPosition dragPositionIn, ref Point itemOffset)
    {
        if (!ReferenceEquals(sender, this))
            AnimationController.Reset();
        var dragPosition = dragPositionIn.ToNewContainer(GlobalRectangle);
        //if (!ReferenceEquals(sender, this)) Debugger.Break();
        DragDelta(sender, item, dragPosition, ref itemOffset);
    }

    void DragDelta(object? sender, object? item, DragPosition dragPositionIn, ref Point itemOffset)
    {
        var dragPosition = dragPositionIn.ToNewContainer(GlobalRectangle);
        SnapDrag(dragPosition, ref itemOffset);
        AnimationController.ShiftAmount =
            ReorderOrientation is Orientation.Horizontal ?
            dragPosition.OriginalItemRect.Width :
            dragPosition.OriginalItemRect.Height;
        AnimationController.StartShiftIndex = AnimationController.IndexOfItemAt(dragPosition.MousePositionToContainer.X, dragPosition.MousePositionToContainer.Y);
    }
    void IMotionDragConnectionReceiver.DragDelta(object? sender, object? item, int senderIndex, DragPosition dragPosition, ref Point itemOffset)
        => DragDelta(sender, item, dragPosition, ref itemOffset);

    void IMotionDragConnectionReceiver.DragLeave(object? sender, object? item, int senderIndex)
    {
        AnimationController.StartShiftIndex = ItemsCount;
    }
    void IMotionDragConnectionReceiver.Drop(object? sender, object? item, int senderIndex, DragPosition dragPosition, DropManager dropManager)
    {
        //var pt = dragPosition.ItemPositionToScreen;
        //var hwnd = Popup.XamlRoot.ContentIslandEnvironment.AppWindowId;
        //AppWindow.GetFromWindowId(hwnd).Move(new() { X = (int)pt.X, Y = (int)pt.Y });
        if (ReferenceEquals(sender, this))
        {
            var newIdx = AnimationController.StartShiftIndex;
            var def = dropManager.GetDeferral();
            //if (SafeContainerFromIndex(ItemDragIndex) is { } curItem && curItem.FindDescendantOrSelf<MotionDragItem>() is { } st)
            //{
            //    var pt = AnimationController.PositionOfItemAtIndex(newIdx);
            //    await st.TemporaryAnimateTranslationAsync(pt.X, pt.Y);
            //}
            AnimationController.Reset();
            newIdx = Math.Min(newIdx, ItemsCount);
            if (newIdx > ItemDragIndex) newIdx--;
            if (newIdx != ItemDragIndex)
            {
                //int i = 0;
                //while (SafeContainerFromIndex(i++)?.FindDescendantOrSelf<MotionDragItem>() is { } st2)
                //{
                //    st2.ResetTranslationImmedietly();
                //}
                OnItemMovingInContainer(ItemDragIndex, newIdx);
                var itemSource = ItemsSource;
                if (itemSource is null)
                {
                    var itemToMove = Items[ItemDragIndex];
                    Items.RemoveAt(ItemDragIndex);
                    Items.Insert(newIdx, itemToMove);
                }
                else if (itemSource is IList list)
                {
                    var itemToMove = list[ItemDragIndex];
                    list.RemoveAt(ItemDragIndex);
                    list.Insert(newIdx, itemToMove);
                    if (list is not INotifyCollectionChanged)
                    {
                        // refresh ItemSource
                        ItemsSource = null;
                        ItemsSource = list;
                    }
                }
                else
                {
                    throw new NotSupportedException("ItemSource must implement IList");
                }
                OnItemMovedInContainer(ItemDragIndex, newIdx);
            }

            // We did want to remove  but I can say false here since we are interacting with the same object
            // and I know what is going on
            dropManager.ShouldItemBeRemovedFromHost = false;
            def.Complete();
        } else
        {
            var newIdx = AnimationController.StartShiftIndex;
            AnimationController.Reset();
            var def = dropManager.GetDeferral();
            //await Task.Delay(1000);
            //if (SafeContainerFromIndex(ItemDragIndex) is { } curItem && curItem.FindDescendantOrSelf<MotionDragItem>() is { } st)
            //{
            //    var pt = AnimationController.PositionOfItemAtIndex(newIdx);
            //    await st.TemporaryAnimateTranslationAsync(pt.X, pt.Y);
            //}
            //int i = 0;
            //while (SafeContainerFromIndex(i++)?.FindDescendantOrSelf<MotionDragItem>() is { } st2)
            //{
            //    st2.ResetTranslationImmedietly();
            //}
            OnItemDroppingFromAnotherContainer(sender, item, senderIndex, newIdx);
            if (newIdx > ItemsCount) newIdx = ItemsCount;
            var itemSource = ItemsSource;
            if (itemSource is null)
            {
                Items.Insert(newIdx, item);
            }
            else if (itemSource is IList list)
            {
                list.Insert(newIdx, item);
                if (list is not INotifyCollectionChanged)
                {
                    // refresh ItemSource
                    ItemsSource = null;
                    ItemsSource = list;
                }
            }
            else
            {
                throw new NotSupportedException("ItemSource must implement IList");
            }
            OnItemDropFromAnotherContainer(sender, item, senderIndex, newIdx);
            dropManager.ShouldItemBeRemovedFromHost = true;
            def.Complete();
        }
    }
    void SnapDrag(DragPosition dragPosition, ref Point itemOffset)
    {
        mousePos = dragPosition.MousePositionToContainer;
        if (mousePos.X > 0 && mousePos.X < ActualWidth && mousePos.Y > 0 && mousePos.Y < ActualHeight)
        {
            if (ReorderOrientation is Orientation.Vertical)
                itemOffset.X -= dragPosition.CumulativeTranslation.X;
            else
                itemOffset.Y -= dragPosition.CumulativeTranslation.Y;
        }
        //if (InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift) is Windows.UI.Core.CoreVirtualKeyStates.Down)
        //    Debugger.Break();
    }
    protected virtual void OnItemDroppingFromAnotherContainer(object? sender, object? item, int senderIndex, int newIndex)
    {

    }
    protected virtual void OnItemDropFromAnotherContainer(object? sender, object? item, int senderIndex, int newIndex)
    {

    }
    protected virtual void OnItemMovingInContainer(int oldIndex, int newIndex)
    {

    }
    protected virtual void OnItemMovedInContainer(int oldIndex, int newIndex)
    {

    }
    private void MotionDragContainer_Unloaded(object sender, RoutedEventArgs e)
    {
        ConnectionContext?.Remove(this);
    }

    private void MotionDragContainer_Loaded(object sender, RoutedEventArgs e)
    {
        ConnectionContext?.Add(this);
    }
}