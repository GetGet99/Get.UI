using Get.UI.MotionDrag;

namespace Get.UI.Windowing;

public readonly struct WindowInfo(XamlRoot xamlRoot)
{
    public nint Hwnd => GlobalContainerRect.GetHwnd(xamlRoot);
}