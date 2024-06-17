global using Windows.Foundation;
global using Windows.Foundation.Collections;
global using Microsoft.UI;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls.Primitives;
global using Microsoft.UI.Xaml.Media;
global using Windows.UI;
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Controls.Primitives;
global using Windows.UI.Xaml.Data;
global using Windows.UI.Xaml.Input;
global using Windows.UI.Xaml.Media;
global using Windows.UI.Xaml.Navigation;
global using Windows.UI.Xaml.Media.Animation;
global using IconSource = Microsoft.UI.Xaml.Controls.IconSource;
global using Windows.System;
global using Windows.UI.Xaml.Markup;
global using Windows.UI.Input;
global using Windows.UI.Xaml.Hosting;
global using Platform = Windows;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Microsoft.Win32;

struct Nothing { }
namespace System.Threading.Tasks
{
    class TaskCompletionSource : TaskCompletionSource<Nothing>
    {
        public void SetResult() => SetResult(default);
    }
}
namespace Windows.Internal
{
    [Guid("f21e14c1-e669-52af-99cd-171eee8e940d")]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IUIContextPartner
    {
        public UIContentRoot UIContentRoot { get; }
        public object WindowContext { [return: MarshalAs(UnmanagedType.IInspectable)] get; } // WindowContext is either CoreWindow, AppWindow, or DesktopWindowContentBridge (or anything that implements Windows.UI.IWindowContextPartner)
    }
    [ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICoreWindowInterop
    {
        IntPtr WindowHandle { get; }
        bool MessageHandled { set; }
    }
    [ComImport, Guid("B74EA3BC-43C1-521F-9C75-E5C15054D78C"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    interface IApplicationWindow_HwndInterop
    {
        Windows.UI.WindowId WindowHandle { get; }
    }
}
namespace WinRT
{
    static class Extension
    {
        public static T As<T>(this object obj)
        {
            Guid guid = typeof(T).GUID;
            Marshal.QueryInterface(Marshal.GetIUnknownForObject(obj),
                ref guid,
                out var corewindowinterop
            );
            return (T)Marshal.GetObjectForIUnknown(corewindowinterop);
        }
    }
}