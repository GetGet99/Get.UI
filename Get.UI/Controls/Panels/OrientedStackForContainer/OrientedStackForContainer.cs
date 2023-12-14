using Get.UI.Controls.Containers;

namespace Get.UI.Controls.Panels;

public partial class OrientedStackForContainer : OrientedStack
{
    public OrientedStackForContainer()
    {
        Loaded += OrientedStackForContainer_Loaded;
        Unloaded += OrientedStackForContainer_Unloaded;
    }

    private void OrientedStackForContainer_Unloaded(object sender, RoutedEventArgs e)
    {
        @event?.Unregister();
    }

    DPCEvent? @event;
    private void OrientedStackForContainer_Loaded(object sender, RoutedEventArgs e)
    {
        var parent = this.FindAscendant<MotionDragContainer>();
        if (parent is not null)
        {
            Orientation = parent.ReorderOrientation;
            @event?.Unregister();
            @event = parent.RegisterPropertyChangedEvent(
                MotionDragContainer.ReorderOrientationProperty,
                delegate
                {
                    Orientation = parent.ReorderOrientation;
                }
            );
        }
    }
    //protected override Size ArrangeOverride(Size finalSize)
    //{
    //    if (IsLoaded)
    //        return base.ArrangeOverride(finalSize);
    //    else
    //        return finalSize;
    //}
    //protected override Size MeasureOverride(Size availableSize)
    //{
    //    if (IsLoaded)
    //        return base.MeasureOverride(availableSize);
    //    else
    //        return new(double.IsInfinity(availableSize.Width) ? availableSize.Width : 100d, double.IsInfinity(availableSize.Height) ? availableSize.Height : 100d);
    //}
}