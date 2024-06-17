#if WINDOWS_UWP
#nullable enable
#endif

namespace Get.UI.MotionDrag;

public interface IMotionDragConnectionReceiver<TItem>
{
    GlobalContainerRect GlobalRectangle { get; }
    bool IsVisibleAt(Point pt);
    void DragEnter(object? sender, TItem item, int senderIndex, DragPosition dragPosition, ref Point itemOffset);
    void DragDelta(object? sender, TItem item, int senderIndex, DragPosition dragPosition, ref Point itemOffset);
    void DragLeave(object? sender, TItem item, int senderIndex);
    void Drop(object? sender, TItem item, int senderIndex, DragPosition dragPosition, DropManager dropManager);
}
public interface IMotionDragConnectionReceiver : IMotionDragConnectionReceiver<object?> { }