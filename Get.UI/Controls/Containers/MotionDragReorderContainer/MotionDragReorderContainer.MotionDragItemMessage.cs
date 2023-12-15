using Get.UI.MotionDrag;
using Microsoft.UI;
using System.Threading.Tasks;

namespace Get.UI.Controls.Containers;

partial class MotionDragContainer
{
    Grid Root => (Grid)GetTemplateChild(nameof(Root));
    Popup Popup => (Popup)GetTemplateChild(nameof(Popup));
    UserControl Display => (UserControl)GetTemplateChild(nameof(Display));
    internal ItemsPresenter ItemPlace => (ItemsPresenter)GetTemplateChild(nameof(ItemPlace));
    Vector3 translation = default;
    Point initmousePos = default;
    Point mousePos = default;
    Point translationRoot = default;
    Point translationXamlRoot = default;
    Rect itemRect;
    int ItemDragIndex = -1;
    object? DraggingObject;
    bool isCanceled = false;

    internal void MotionDragItemManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        e.Handled = true;
        isCanceled = false;
        if (sender is not UIElement ele) return;
        Display.Width = ele.ActualSize.X;
        Display.Height = ele.ActualSize.Y;

        var eleVisual = ElementCompositionPreview.GetElementVisual(ele);
        var compositor = eleVisual.Compositor;
        ElementCompositionPreview.SetElementChildVisual(
            Display,
            compositor.CreateRedirectVisual(eleVisual)
        );
        eleVisual.IsVisible = false;
        translationRoot = ele.TransformToVisual(Root).TransformPoint(default);
        initmousePos = e.Position.Add(translationRoot);
        translationXamlRoot = ele.TransformToVisual(Root.XamlRoot.Content).TransformPoint(e.Position);
        itemRect = new(translationRoot, new Size(ele.ActualSize.X, ele.ActualSize.Y));
        _globalRectangle = GlobalContainerRect.GetFromContainer(this);
        var idx = IndexFromContainer(ele);
        if (idx is -1) idx = IndexFromContainer(ele.FindAscendant<ContentPresenter>());
        if (idx is -1) Debugger.Break();
        AnimationController.Reset();
        AnimationController.StartRemoveIndex = ItemDragIndex = idx;
        DraggingObject = ItemFromContainer(ContainerFromIndex(idx));

        Popup.Translation = new((float)translationRoot.X, (float)translationRoot.Y, 0);
        Popup.IsOpen = true;
        set = false;
    }
    bool set;
    internal void MotionDragItemManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        e.Handled = true;
        if (isCanceled) return;
        if (WinWrapper.Input.Keyboard.IsKeyDown(WinWrapper.Input.VirtualKey.ESCAPE))
        {
            isCanceled = true;
            Popup.IsOpen = false;
            if (sender is UIElement ele)
            {
                var eleVisual = ElementCompositionPreview.GetElementVisual(ele);
                eleVisual.IsVisible = true;
            }
            AnimationController.Reset();
            return;
        }
        DragPosition dp = new(GlobalRectangle, itemRect, initmousePos, e.Cumulative.Translation);
        if (!set)
        {
            var mousePos = dp.MousePositionToScreen;
            var window = WinWrapper.Windowing.Window.FromLocation((int)mousePos.X, (int)mousePos.Y);
            //if (WinWrapper.Input.Keyboard.IsShiftDown) Debugger.Break();
            if (window.Class.Name is
#if WINDOWS_UWP
                "Xaml_WindowedPopupClass"
#else
                "Microsoft.UI.Content.PopupWindowSiteBridge"
#endif
            )
            {
                window.SetTopMost();
                window[WinWrapper.Windowing.WindowExStyles.Layered] = true;
                window[WinWrapper.Windowing.WindowExStyles.Transparent] = true;
                set = true;
            }
        }
        translation = new Vector3(
            (float)(e.Cumulative.Translation.X + translationRoot.X),
            (float)(e.Cumulative.Translation.Y + translationRoot.Y),
        0);
        var localTranslation = translation;
        ConnectionContext?.DragEvent(this, DraggingObject, ItemDragIndex, dp, ref localTranslation);
        Popup.Translation = localTranslation;
        //var hwnd = Popup.XamlRoot.ContentIslandEnvironment.AppWindowId;
        //System.Drawing.Point pt = default;
        //_ = PInvoke.MapWindowPoints(new((nint)hwnd.Value), default, &pt, 1);
        //var scale = Popup.XamlRoot.RasterizationScale;
        //testValue.Text = $"{(e.Cumulative.Translation.X + translationXamlRoot.X) * scale + pt.X}, {(e.Cumulative.Translation.Y + translationXamlRoot.Y) * scale + pt.Y}";
    }

    internal async void MotionDragItemManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        e.Handled = true;
        if (isCanceled)
        {
            isCanceled = false;
            return;
        }
        var dropManager = new DropManager();
        ConnectionContext?.DropEvent(this, DraggingObject, ItemDragIndex, new(GlobalRectangle, itemRect, initmousePos, e.Cumulative.Translation), dropManager);
        await dropManager.WaitForDeferralAsync();
        if (sender is UIElement ele)
        {
            var eleVisual = ElementCompositionPreview.GetElementVisual(ele);
            eleVisual.IsVisible = true;
        }
        //var window = WinWrapper.Windowing.Window.ActiveWindow;
        //window.SetTopMost();
        Popup.IsOpen = false;
        //window.Activate();
        if (dropManager.ShouldItemBeRemovedFromHost)
        {
            var itemSource = ItemsSource;
            if (itemSource is null)
            {
                var itemToMove = Items[ItemDragIndex];
                Items.RemoveAt(ItemDragIndex);
            }
            else if (itemSource is IList list)
            {
                list.RemoveAt(ItemDragIndex);
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
        }
        AnimationController.Reset();

        //var hwnd = Popup.XamlRoot.ContentIslandEnvironment.AppWindowId;
        //System.Drawing.Point pt = default;
        //_ = PInvoke.MapWindowPoints(new((nint)hwnd.Value), default, &pt, 1);
        //var scale = Popup.XamlRoot.RasterizationScale;
        //var pos = new System.Drawing.Point(
        //    (int)((e.Cumulative.Translation.X //+ translationXamlRoot.X
        //                                      ) * scale + pt.X),
        //    (int)((e.Cumulative.Translation.Y //+ translationXamlRoot.Y
        //                                      ) * scale + pt.Y)
        //    );
        //AppWindow.GetFromWindowId(hwnd).Move(new() { X = pos.X, Y = pos.Y });
    }
}