using Get.UI.Controls.Containers;
using Get.UI.Controls.Panels;

namespace Get.UI.Extensions;
[AttachedProperty(typeof(bool), "IsEnabled", DependencyObjectType: typeof(FrameworkElement), GenerateLocalOnPropertyChangedMethod = true)]
public static partial class AutoOrientationPropertyExtension
{
    public static event Func<DependencyObject, DependencyProperty>? OrientationPropertyExternalLookup;
    static Dictionary<DependencyObject, (RoutedEventHandler loaded, RoutedEventHandler unload, DPCEvent? parent, DPCEvent? self)> Events = new();
    static partial void OnIsEnabledChanged(FrameworkElement obj, bool oldValue, bool newValue)
    {
        if (oldValue == newValue) return;
        if (newValue)
        {
            RegisterEvent(obj);
        } else
        {
            UnregisterEvent(obj);
        }
    }
    static void RegisterEvent(FrameworkElement self)
    {
        UnregisterEvent(self);
        void Loaded(object _, RoutedEventArgs _1) => InnerRegisterEvent(self);
        void Unloaded(object _, RoutedEventArgs _1) => InnerUnregisterEvent(self);
        Events[self] = (Loaded, Unloaded, null, null);
        self.Loaded += Loaded;
        self.Unloaded += Unloaded;
        if (self.IsLoaded)
            // run immedietly
            InnerRegisterEvent(self);
    }
    static void UnregisterEvent(FrameworkElement self)
    {
        if (Events.Remove(self, out var evs))
        {
            self.Loaded -= evs.loaded;
            self.Unloaded -= evs.unload;
            evs.self?.Unregister();
            evs.parent?.Unregister();
        }
    }
    static void InnerUnregisterEvent(FrameworkElement self)
    {
        if (Events.TryGetValue(self, out var evs))
        {
            evs.self?.Unregister();
            evs.parent?.Unregister();

            // reset to null as parent might change
            evs.self = null;
            evs.parent = null;
            Events[self] = evs;
        }
    }
    static void InnerRegisterEvent(FrameworkElement self)
    {
        if (Events.TryGetValue(self, out var evs))
        {
            if (evs.self is not null && evs.parent is not null)
            {
                evs.self.Register();
                evs.parent.Register();
                return;
            } else if (evs.self is null != evs.parent is null)
            {
                UnregisterEvent(self);
                RegisterEvent(self);
                return;
            }
        }
        if (FindOrientationProperty(self) is not { } senderProp) throw new ArgumentException($"Cannot find the orientation property for object {self}");
        if (FirstNotNullOrNull(self.FindAscendants(), potentialParent =>
        {
            var prop = FindOrientationProperty(potentialParent);
            if (prop is null) return null;
            return new Tuple<DependencyObject, DependencyProperty>(potentialParent, prop);
        }) is not { } parentAndProp) throw new ArgumentException($"Cannot find the parent with orientation property");
        var (parent, parentProp) = parentAndProp;
        var selfEv = self.RegisterPropertyChangedEvent(senderProp, (senderAsSelf, dp) =>
        {
            var o = (Orientation)senderAsSelf.GetValue(dp);
            if ((Orientation)parent.GetValue(parentProp) != o)
                parent.SetValue(parentProp, o);
        });
        var parentEv = parent.RegisterPropertyChangedEvent(parentProp, (parentAsSelf, dp) =>
        {
            var o = (Orientation)parentAsSelf.GetValue(dp);
            if ((Orientation)self.GetValue(senderProp) != o)
                self.SetValue(senderProp, o);
        });
        Events[self] = Events[self] with { self = selfEv, parent = parentEv };
    }


    static DependencyProperty? FindOrientationProperty(DependencyObject sender)
    {
        return sender switch
        {
            ItemsStackPanel => ItemsStackPanel.OrientationProperty,
            StackPanel => StackPanel.OrientationProperty,
            OrientedStack => OrientedStack.OrientationProperty,
            MotionDragContainer => MotionDragContainer.ReorderOrientationProperty,
            _ => OrientationPropertyExternalLookup?.Invoke(sender) ?? null
        };
    }
    static TReturn? FirstNotNullOrNull<TReturn, TSource>(IEnumerable<TSource> t, Func<TSource, TReturn?> func) where TReturn : class
    {
        foreach (var item in t)
        {
            if (func(item) is TReturn output) return output;
        }
        return null;
    }
}