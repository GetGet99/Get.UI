#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;
using winmdroot = global::Windows.Win32;
namespace Windows.Win32
{
    partial class PInvoke
    {
        static PInvoke()
        {
            var user32module = GetModuleHandle("user32.dll");
            if (user32module == default)
            {
                user32module = GetModuleHandle("ext-ms-win-rtcore-webview-l1-1-0.dll");
            }
            if (user32module == default) Environment.FailFast("Failed to obtain user32 apis");
            _MapWindowsPoint =
                Marshal.GetDelegateForFunctionPointer<MapWindowPointsDelegate>(
                    GetProcAddress(user32module, nameof(MapWindowPoints))
                );
        }
        /// <summary>The MapWindowPoints function converts (maps) a set of points from a coordinate space relative to one window to a coordinate space relative to another window.</summary>
        /// <param name="hWndFrom">A handle to the window from which points are converted. If this parameter is <b>NULL</b> or HWND_DESKTOP, the points are presumed to be in screen coordinates.</param>
        /// <param name="hWndTo">A handle to the window to which points are converted. If this parameter is <b>NULL</b> or HWND_DESKTOP, the points are converted to screen coordinates.</param>
        /// <param name="lpPoints">A pointer to an array of <a href="https://docs.microsoft.com/windows/win32/api/windef/ns-windef-point">POINT</a> structures that contain the set of points to be converted. The points are in device units. This parameter can also point to a <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> structure, in which case the <i>cPoints</i> parameter should be set to 2.</param>
        /// <param name="cPoints">The number of <a href="https://docs.microsoft.com/windows/win32/api/windef/ns-windef-point">POINT</a> structures in the array pointed to by the <i>lpPoints</i> parameter.</param>
        /// <returns>
        /// <para>If the function succeeds, the low-order word of the return value is the number of pixels added to the horizontal coordinate of each source point in order to compute the horizontal coordinate of each destination point. (In addition to that, if precisely one of <i>hWndFrom</i> and <i>hWndTo</i> is mirrored, then each resulting horizontal coordinate is multiplied by -1.) The high-order word is the number of pixels added to the vertical coordinate of each source point in order to compute the vertical coordinate of each destination point. If the function fails, the return value is zero. Call <a href="https://docs.microsoft.com/windows/desktop/api/errhandlingapi/nf-errhandlingapi-setlasterror">SetLastError</a> prior to calling this method to differentiate an error return value from a legitimate "0" return value.</para>
        /// </returns>
        /// <remarks>
        /// <para>If <i>hWndFrom</i> or <i>hWndTo</i> (or both) are mirrored windows (that is, have <b>WS_EX_LAYOUTRTL</b> extended style) and precisely two points are passed in <i>lpPoints</i>, <b>MapWindowPoints</b> will interpret those two points as a <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> and possibly automatically swap the left and right fields of that rectangle to ensure that left is not greater than right. If any number of points other than 2 is passed in <i>lpPoints</i>, then <b>MapWindowPoints</b> will correctly map the coordinates of each of those points separately, so if you pass in a pointer to an array of more than one rectangle in <i>lpPoints</i>, the new rectangles may get their left field greater than right. Thus, to guarantee the correct transformation of rectangle coordinates, you must call <b>MapWindowPoints</b> with one <b>RECT</b> pointer at a time, as shown in the following example:</para>
        /// <para></para>
        /// <para>This doc was truncated.</para>
        /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-mapwindowpoints#">Read more on docs.microsoft.com</see>.</para>
        /// </remarks>
        internal static unsafe int MapWindowPoints(winmdroot.Foundation.HWND hWndFrom, winmdroot.Foundation.HWND hWndTo, global::System.Drawing.Point* lpPoints, uint cPoints)
            => _MapWindowsPoint.Invoke(hWndFrom, hWndTo, lpPoints, cPoints);
        static MapWindowPointsDelegate _MapWindowsPoint;
        unsafe delegate int MapWindowPointsDelegate(winmdroot.Foundation.HWND hWndFrom, winmdroot.Foundation.HWND hWndTo, global::System.Drawing.Point* lpPoints, uint cPoints);
    }
}