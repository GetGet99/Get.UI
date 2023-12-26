namespace Get.UI.Controls.Containers;
partial class MotionDragItem
{
    void InitManipulation()
    {
        ManipulationStarted += (o, e) => Owner?.MotionDragItemManipulationStarted(o, e);
        ManipulationDelta += (o, e) => Owner?.MotionDragItemManipulationDelta(o, e);
        ManipulationCompleted += (o, e) => Owner?.MotionDragItemManipulationCompleted(o, e);
    }
}
