#if WINDOWS_UWP
#nullable enable
#endif
namespace Get.UI.MotionDrag;

public class MotionDragConnectionContext<TItem>
{
    readonly List<IMotionDragConnectionReceiver<TItem>> Receivers = [], QueuedRemoveReceivers = [];
    internal void Add(IMotionDragConnectionReceiver<TItem> control) => Receivers.Add(control);
    internal bool Remove(IMotionDragConnectionReceiver<TItem> control) => Receivers.Remove(control);
    public event MotionDragConnectionDraggingOutside? DraggingOutside;
    public event MotionDragConnectionDroppedOutside? DroppedOutside;
    IMotionDragConnectionReceiver<TItem>? CurrentReceiver;
    internal void DragEvent(object? sender, TItem item, int senderIndex, DragPosition dragPosition, ref Vector3 itemOffset)
    {
        var mousePos = dragPosition.MousePositionToScreen;
    Start:
        if (CurrentReceiver is null)
        {
            foreach (var receiver in Receivers)
            {
                var globalRect = receiver.GlobalRectangle;
                if (!globalRect.IsValid)
                {
                    QueuedRemoveReceivers.Add(receiver);
                    continue;
                }
                var rect = globalRect.ContainerRectToScreen;
                if (rect.Contains(mousePos) && receiver.IsVisibleAt(mousePos.Subtract(receiver.GlobalRectangle.WindowPosOffset)))
                {
                    CurrentReceiver = receiver;
                    Point _offset = default;
                    CurrentReceiver.DragEnter(sender, item, senderIndex, dragPosition, ref _offset);
                    itemOffset = new(
                        (float)_offset.X + itemOffset.X,
                        (float)_offset.Y + itemOffset.Y,
                        itemOffset.Z
                    );
                    goto Finalize;
                }
            }
            DraggingOutside?.Invoke(sender, item, dragPosition);
        }
        else
        {
            var globalRect = CurrentReceiver.GlobalRectangle;
            if (!globalRect.IsValid)
            {
                Receivers.Remove(CurrentReceiver);
                CurrentReceiver = null;
                goto Start;
            }
            var rect = globalRect.ContainerRectToScreen;
            if (rect.Contains(mousePos) && CurrentReceiver.IsVisibleAt(mousePos.Subtract(CurrentReceiver.GlobalRectangle.WindowPosOffset)))
            {
                Point _offset = default;
                CurrentReceiver.DragDelta(sender, item, senderIndex, dragPosition, ref _offset);
                itemOffset += _offset.AsVector3();
            }
            else
            {
                CurrentReceiver.DragLeave(sender, item, senderIndex);
                CurrentReceiver = null;
                DragEvent(sender, item, senderIndex, dragPosition, ref itemOffset);
            }
        }
    Finalize:
        foreach (var receiver in QueuedRemoveReceivers)
        {
            Receivers.Remove(receiver);
        }
        QueuedRemoveReceivers.Clear();
    }
    internal void DropEvent(object? sender, TItem item, int senderIndex, DragPosition dragPosition, DropManager dropManager)
    {
        if (CurrentReceiver is not null)
            CurrentReceiver.Drop(sender, item, senderIndex, dragPosition, dropManager);
        else
            DroppedOutside?.Invoke(sender, item, dragPosition, dropManager);
    }
    internal void CancelDragEvent(object? sender, TItem item, int senderIndex)
    {
        CurrentReceiver?.DragLeave(sender, item, senderIndex);
        CurrentReceiver = null;
    }
    public static void UnsafeAdd(MotionDragConnectionContext<TItem> reference, IMotionDragConnectionReceiver<TItem> receiver)
        => reference.Add(receiver);
    public static void UnsafeRemove(MotionDragConnectionContext<TItem> reference, IMotionDragConnectionReceiver<TItem> receiver)
        => reference.Remove(receiver);
    public static void UnsafeSendDragEvent() { }
}
public class MotionDragConnectionContext : MotionDragConnectionContext<object?> { }
