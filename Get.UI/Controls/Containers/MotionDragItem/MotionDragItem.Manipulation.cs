namespace Get.UI.Controls.Containers;
partial class MotionDragItem
{
    void InitManipulation()
    {

        ManipulationStarted += (o, e) => Owner?.MotionDragItemManipulationStarted(o, e);
        ManipulationDelta += (o, e) => Owner?.MotionDragItemManipulationDelta(o, e);
        ManipulationCompleted += (o, e) => Owner?.MotionDragItemManipulationCompleted(o, e);
        //RootElement.ManipulationStarting += (_, e) =>
        //{
        //    if (ReorderTranslationAnimation)
        //    {
        //        if (!(Owner?.IsReadyForNextReorderManipulation ?? false))
        //            e.Mode = ManipulationModes.None;
        //    }
        //};
        //RootElement.ManipulationStarted += (_, e) =>
        //{
        //    if (ReorderTranslationAnimation)
        //    {
        //        Owner?.MotionDragItemManipulationStart(this);
        //    }
        //};
        //RootElement.ManipulationDelta += (_, e) =>
        //{
        //    if (ReorderTranslationAnimation)
        //    {
        //        RootTransform.TranslateX += e.Delta.Translation.X;
        //        RootTransform.TranslateY += e.Delta.Translation.Y;
        //        Owner?.MotionDragItemManipulationDelta(this, e.Delta.Translation.X, e.Delta.Translation.Y);
        //    }
        //};
        //RootElement.ManipulationCompleted += (_, e) =>
        //{
        //    if (ReorderTranslationAnimation)
        //    {
        //        //TranslationResetStoryboard.Begin();
        //        Owner?.MotionDragItemManipulationEnd(this);
        //    }
        //};
    }
    internal void OnReorderOrientationUpdated()
    {
        // owner.ReorderOrientation
    }

    //protected override async void OnPointerPressed(PointerRoutedEventArgs e)
    //{
    //    CapturePointer(e.Pointer);
    //    var pt = e.GetCurrentPoint(this).Position;
    //    ptX = pt.X;
    //    ptY = pt.Y;
    //    if (ReorderTranslationAnimation)
    //    {
    //        Owner?.MotionDragItemManipulationStart(this);
    //    }
    //    base.OnPointerPressed(e);
    //}
    //double ptX, ptY;
    //protected override async void OnPointerMoved(PointerRoutedEventArgs e)
    //{
    //    base.OnPointerMoved(e);
    //    var currentPoint = e.GetCurrentPoint(this);
    //    if (!currentPoint.IsInContact) return;
    //    if (e.KeyModifiers == VirtualKeyModifiers.Shift)
    //    {
    //        var container = (UIElement)Owner.ContainerFromItem(Owner.ObjectFromSRI(this));
    //        var a = container.StartDragAsync(e.GetCurrentPoint(container));

    //        while (a.Status is not AsyncStatus.Completed)
    //        {
    //            await Task.Delay(1000);
    //            if (!InputKeyboardSource
    //                .GetKeyStateForCurrentThread(VirtualKey.Shift)
    //                .HasFlag(CoreVirtualKeyStates.Down))
    //            {
    //                a.Cancel();
    //                CapturePointer(e.Pointer);
    //                break;
    //            }
    //        }
    //    } else
    //    {
    //        if (ReorderTranslationAnimation)
    //        {
    //            var deltaX = currentPoint.Position.X - ptX;
    //            var deltaY = currentPoint.Position.Y - ptY;
    //            if (ManipulationMode is ManipulationModes.TranslateX)
    //                deltaY = 0;
    //            else
    //                deltaX = 0;
    //            RootTransform.TranslateX += deltaX;
    //            RootTransform.TranslateY += deltaY;
    //            Owner?.MotionDragItemManipulationDelta(this, deltaX, deltaY);
    //            ptX = currentPoint.Position.X;
    //            ptY = currentPoint.Position.Y;
    //        }
    //    }
    //}
    //protected override void OnPointerReleased(PointerRoutedEventArgs e)
    //{
    //    if (ReorderTranslationAnimation)
    //    {
    //        //TranslationResetStoryboard.Begin();
    //        Owner?.MotionDragItemManipulationEnd(this);
    //    }
    //    base.OnPointerReleased(e);
    //}
    //partial void OnReorderTranslationAnimationChanged(bool oldValue, bool newValue)
    //{
    //    if (oldValue == newValue) return;
    //    OnReorderOrientationUpdated();
    //}
}
