using Get.UI.MotionDrag;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Get.UI.ApplicationModel.Tabbed;
[DependencyProperty<bool>("AllowEmptyWindow", DefaultValueExpression = "false")]
public abstract partial class TabbedApplicationManager<T> : TabbedApplicationManager
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
        _windows.Add(model);
        return model;
    }

    public new TabbedWindowModel<T> CreateNewWindow()
    {
#if WINDOWS_UWP
        throw new NotImplementedException();
#else
        var window = new Windowing.Window(new(), isSettingRootContentAllowed: false);
        var model = new TabbedWindowModel<T>(this, window);
        window.PlatformWindow.ExtendsContentIntoTitleBar = true;
        window.PlatformWindow.SystemBackdrop = SystemBackdrop;
        window.PlatformWindow.Content = new TabbedPage(this, model);
        model.TabItems.CollectionChanged += delegate
        {
            if (model.TabItems.Count is 0 && !AllowEmptyWindow)
            {
                model.Window.Close();
            }
        };
        window.PlatformWindow.Closed += delegate
        {
            if (model.TabItems.Count > 0)
                model.TabItems.Clear();
            _windows.Remove(model);
        };
        window.PlatformWindow.AppWindow.Closing += delegate
        {
            
        };
        _windows.Add(model);
        return model;
#endif
    }
    internal override TabbedWindowModel CreateNewWindowImplement()
        => CreateNewWindow();
    internal override TabbedWindowModel CreateNewWindowImplement(object tabModel)
        => CreateNewWindow((T)tabModel);
    protected abstract void OnWindowClosing(Windowing.Window window);
    protected abstract T CreateNewTab(object? parameter);
    internal override void InternalAddTab(TabbedWindowModel model, object? parameter)
    {
        var newTab = CreateNewTab(parameter);
        var typedModel = ((TabbedWindowModel<T>)model);
        typedModel.TabItems.Add(newTab);
        typedModel.PrimarySelectedIndex = typedModel.TabItems.Count - 1;
    }

}
class ActionCommand(Action<object?> act) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => act(parameter);
}
#if WINDOWS_UWP
#else
[DependencyProperty<SystemBackdrop>("SystemBackdrop")]
#endif
[DependencyProperty<DataTemplate>("WindowTemplate")]
[DependencyProperty<DataTemplate>("ItemTemplate")]
[DependencyProperty<DataTemplate>("ContentTemplate")]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext", GenerateLocalOnPropertyChangedMethod = true)]
public abstract partial class TabbedApplicationManager : DependencyObject
{
    internal TabbedApplicationManager(IReadOnlyList<TabbedWindowModel> windows) {
        Windows = windows;
        ConnectionContext = new();
        WindowTemplate = DefaultTemplates.Singleton.TabWindowTemplate;
        ItemTemplate = DefaultTemplates.Singleton.TabItemTemplate;
        ContentTemplate = DefaultTemplates.Singleton.TabContentTemplate;
    }
    public IReadOnlyList<TabbedWindowModel> Windows { get; }
    public TabbedWindowModel CreateNewWindow() => CreateNewWindowImplement();
    internal abstract TabbedWindowModel CreateNewWindowImplement();
    internal abstract TabbedWindowModel CreateNewWindowImplement(object tabModel);
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
#if WINDOWS_UWP
        throw new NotImplementedException();
#else
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
#endif
    }
    internal abstract void InternalAddTab(TabbedWindowModel model, object? parameter);
}