namespace Get.UI.MotionDrag;

public class MotionDragConnectionContext
{
    readonly List<IMotionDragConnectionReceiver> Receivers = new();
    internal void Add(IMotionDragConnectionReceiver control) => Receivers.Add(control);
    internal bool Remove(IMotionDragConnectionReceiver control) => Receivers.Remove(control);
    public event MotionDragConnectionDraggingOutside? DraggingOutside;
    public event MotionDragConnectionDroppedOutside? DroppedOutside;
    IMotionDragConnectionReceiver? CurrentReceiver;
    internal void DragEvent(object? sender, object? item, int senderIndex, DragPosition dragPosition, ref Vector3 itemOffset)
    {
        var mousePos = dragPosition.MousePositionToScreen;
        if (CurrentReceiver is null)
        {
            foreach (var receiver in Receivers)
            {
                var rect = receiver.GlobalRectangle.ContainerRectToScreen;
                if (WinWrapper.Input.Keyboard.IsShiftDown) Debugger.Break();
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
                    return;
                }
            }
            DraggingOutside?.Invoke(sender, item, dragPosition);
        } else
        {
            var rect = CurrentReceiver.GlobalRectangle.ContainerRectToScreen;
            if (rect.Contains(mousePos) && CurrentReceiver.IsVisibleAt(mousePos.Subtract(CurrentReceiver.GlobalRectangle.WindowPosOffset)))
            {
                Point _offset = default;
                CurrentReceiver.DragDelta(sender, item, senderIndex, dragPosition, ref _offset);
                itemOffset += _offset.AsVector3();
            } else
            {
                CurrentReceiver.DragLeave(sender, item, senderIndex);
                CurrentReceiver = null;
                DragEvent(sender, item, senderIndex, dragPosition, ref itemOffset);
            }
        }
    }
    internal void DropEvent(object? sender, object? item, int senderIndex, DragPosition dragPosition, DropManager dropManager)
    {
        if (CurrentReceiver is not null)
            CurrentReceiver.Drop(sender, item, senderIndex, dragPosition, dropManager);
        else
            DroppedOutside?.Invoke(sender, item, dragPosition, dropManager);
    }
    internal void CancelDragEvent(object? sender, object? item, int senderIndex)
    {
        CurrentReceiver?.DragLeave(sender, item, senderIndex);
        CurrentReceiver = null;
    }
    public static void UnsafeAdd(MotionDragConnectionContext reference, IMotionDragConnectionReceiver receiver)
        => reference.Add(receiver);
    public static void UnsafeRemove(MotionDragConnectionContext reference, IMotionDragConnectionReceiver receiver)
        => reference.Remove(receiver);
    public static void UnsafeSendDragEvent() { }
}