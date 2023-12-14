using System.Threading.Tasks;
namespace Get.UI.MotionDrag;

public interface IMotionDragConnectionReceiver
{
    GlobalContainerRect GlobalRectangle { get; }
    bool IsVisibleAt(Point pt);
    void DragEnter(object? sender, object? item, int senderIndex, DragPosition dragPosition, ref Point itemOffset);
    void DragDelta(object? sender, object? item, int senderIndex, DragPosition dragPosition, ref Point itemOffset);
    void DragLeave(object? sender, object? item, int senderIndex);
    void Drop(object? sender, object? item, int senderIndex, DragPosition dragPosition, DropManager dropManager);
}