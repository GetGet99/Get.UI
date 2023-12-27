using System.ComponentModel;

namespace Get.UI.Windowing;

public abstract partial class Window
{
    public abstract UIElement RootContent { get; set; }
    public abstract XamlRoot XamlRoot { get; }
    public abstract object PlatformWindow { get; }
    public abstract double TitleBarLeftInset { get; }
    public abstract double TitleBarRightInset { get; }
    public abstract Rect Bounds { get; set; }
    public abstract Rect ClientBounds { get; set; }
    public abstract void Close();
    public abstract nint WindowHandle { get; }
    public abstract bool ExtendsContentIntoTitleBar { get; set; }
    public abstract event DefferalCanceledEventHandler Closing;
    public abstract event EventHandler Closed;
    public abstract void Activate();
}