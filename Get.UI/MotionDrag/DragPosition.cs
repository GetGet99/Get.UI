namespace Get.UI.MotionDrag;

public readonly record struct DragPosition(GlobalContainerRect ContainerPosition, Rect OriginalItemRect, Point MouseOffset, Point CumulativeTranslation) {
    //public GlobalContainerRect GlobalItemPosition => new( // ContainerPosition.WindowPosOffset, // OriginalItemRect.WithLocation( // OriginalItemRect.GetLocation() // .Add(ContainerPosition.ContainerRectToWindow.GetLocation()) // ), // ContainerPosition.RasterizationScale);
    public Point MousePositionToContainer => MouseOffset.Add(CumulativeTranslation);
    public Point MousePositionToScreen => ContainerPosition.PointToScreen(MousePositionToContainer);
    public Point ItemPositionToContainer => CumulativeTranslation;
    public Point ItemPositionToScreen => ContainerPosition.PointToScreen(OriginalItemRect.GetLocation().Add(CumulativeTranslation));
    public DragPosition ToNewContainer(GlobalContainerRect newContainer) => new( newContainer, ContainerPosition.ToNewContainer(newContainer, OriginalItemRect), ContainerPosition.ScaleToNewContainer(newContainer, MouseOffset), ContainerPosition.ToNewContainer(newContainer, CumulativeTranslation) );
}
