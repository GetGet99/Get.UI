using Get.UI.MotionDrag;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Get.UI.ApplicationModel.Tabbed;
public partial class TabbedApplicationManager<T> : TabbedApplicationManager
{
    static T2 Assign<T2>(T2 value, out T2 variable)
    {
        variable = value;
        return value;
    }
    public TabbedApplicationManager() : base(
        Assign(
            new ReadOnlyObservableCollection<TabbedWindowModel<T>>(
            Assign(new ObservableCollection<TabbedWindowModel<T>>(), out var a)),
            out var b
        ))
    {
        _windows = a;
        Windows = b;
    }

    readonly ObservableCollection<TabbedWindowModel<T>> _windows;

    public event PropertyChangedEventHandler? PropertyChanged;
    public new ReadOnlyObservableCollection<TabbedWindowModel<T>> Windows { get; }
    public TabbedWindowModel<T> CreateNewWindow(T newTab)
    {
        var model = CreateNewWindow();
        model.TabItems.Add(newTab);
        return model;
    }
    public new TabbedWindowModel<T> CreateNewWindow()
    {
#if WINDOWS_UWP
#else
        var window = new Windowing.Window(new(), isSettingRootContentAllowed: false);
        var model = new TabbedWindowModel<T>(this, window);
        window.PlatformWindow.SystemBackdrop = SystemBackdrop;
        window.PlatformWindow.Content = new TabbedPage(this, model);
        _windows.Add(model);
        return model;
#endif
    }
    protected override TabbedWindowModel CreateNewWindowImplement()
        => CreateNewWindow();
    protected override TabbedWindowModel CreateNewWindowImplement(object tabModel)
        => CreateNewWindow((T)tabModel);
}
#if WINDOWS_UWP
#else
[DependencyProperty<SystemBackdrop>("SystemBackdrop")]
#endif
[DependencyProperty<DataTemplate>("WindowTemplate")]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext", GenerateLocalOnPropertyChangedMethod = true)]
public abstract partial class TabbedApplicationManager : DependencyObject
{
    internal TabbedApplicationManager(IReadOnlyList<TabbedWindowModel> windows) {
        Windows = windows;
        ConnectionContext = new();
    }
    public IReadOnlyList<TabbedWindowModel> Windows { get; }
    public TabbedWindowModel CreateNewWindow() => CreateNewWindowImplement();
    protected abstract TabbedWindowModel CreateNewWindowImplement();
    protected abstract TabbedWindowModel CreateNewWindowImplement(object tabModel);
    partial void OnConnectionContextChanged(MotionDragConnectionContext oldValue, MotionDragConnectionContext newValue)
    {
        if (oldValue is not null)
        {
            oldValue.DroppedOutside -= ConnectionContext_DroppedOutside;
        }
        if (newValue is not null)
        {
            newValue.DroppedOutside += ConnectionContext_DroppedOutside;
        }
    }

    private void ConnectionContext_DroppedOutside(object? sender, object? item, DragPosition dragPosition, DropManager dropManager)
    {
        dropManager.ShouldItemBeRemovedFromHost = true;
        if (item is not null)
        {
            var model = CreateNewWindowImplement(item);
            var mousePos = dragPosition.MousePositionToScreen;
            if (sender is UIElement ele)
                model.Window.Bounds =
                        WinWrapper.Windowing.Window
                        .FromWindowHandle(GlobalContainerRect.GetHwnd(ele.XamlRoot))
                        .Bounds with
                        { X = (int)mousePos.X, Y = (int)mousePos.Y };
            else
                model.Window.Bounds =
                    model.Window.Bounds with { X = (int)mousePos.X, Y = (int)mousePos.Y };
            model.Window.PlatformWindow.Activate();
        }
    }
}