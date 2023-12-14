using System.Runtime.CompilerServices;
namespace Get.UI.Extensions;
class DPCEvent(DependencyObject Object, DependencyProperty Property, DependencyPropertyChangedCallback Callback)
{
    bool isRegistered = false;
    long token;
    public bool IsRegistered
    {
        get => isRegistered;
        set { if (value) Register(); else Unregister(); }
    }
    public void Register()
    {
        if (isRegistered) return;
        isRegistered = true;
        token = Object.RegisterPropertyChangedCallback(Property, Callback);
    }
    public void Unregister()
    {
        if (!isRegistered) return;
        Object.UnregisterPropertyChangedCallback(Property, token);
        token = default;
        isRegistered = false;
    }
}
static class DPCEventExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DPCEvent RegisterPropertyChangedEvent(this DependencyObject Object, DependencyProperty Property, DependencyPropertyChangedCallback Callback)
    {
        var ev = new DPCEvent(Object, Property, Callback);
        ev.Register();
        return ev;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DPCEvent CreatePropertyChangedEvent(this DependencyObject Object, DependencyProperty Property, DependencyPropertyChangedCallback Callback)
    {
        var ev = new DPCEvent(Object, Property, Callback);
        return ev;
    }
}