namespace Get.UI.MotionDrag;
public delegate void MotionDragConnectionDraggingOutside(object? sender, object? item, DragPosition dragPosition);
public delegate void MotionDragConnectionDroppedOutside(object? sender, object? item, DragPosition dragPosition, DropManager dropManager);