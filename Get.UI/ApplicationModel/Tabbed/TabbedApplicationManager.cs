using Get.UI.MotionDrag;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.WindowManagement;

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
    public async Task<TabbedWindowModel<T>> CreateNewWindowAsync(T newTab)
    {
        var model = await CreateNewWindowAsync();
        model.TabItems.Add(newTab);
        _windows.Add(model);
        return model;
    }

    public async Task<TabbedWindowModel<T>> CreateNewWindowAsync()
    {
        var window = await Windowing.Window.CreateAsync();
        return MakeTabWindowModel(window);
    }
    public TabbedWindowModel<T> MakeTabWindowModel(Windowing.Window newEmptyWindow, T newTab)
    {
        var model = MakeTabWindowModel(newEmptyWindow);
        model.TabItems.Add(newTab);
        _windows.Add(model);
        return model;
    }
    public TabbedWindowModel<T> MakeTabWindowModel(Windowing.Window newEmptyWindow)
    {
        var model = new TabbedWindowModel<T>(this, newEmptyWindow);
        newEmptyWindow.ExtendsContentIntoTitleBar = true;
#if WINDOWS_UWP
#else
        var platformWindow = (Microsoft.UI.Xaml.Window)newEmptyWindow.PlatformWindow;
        platformWindow.SystemBackdrop = SystemBackdrop;
#endif
        newEmptyWindow.RootContent = new TabbedPage(this, model);
        model.TabItems.CollectionChanged += delegate
        {
            if (model.TabItems.Count is 0 && !AllowEmptyWindow)
            {
                model.Window.Close();
            }
        };
        newEmptyWindow.Closed += delegate
        {
            //if (model.TabItems.Count > 0)
            //    model.TabItems.Clear();
            _windows.Remove(model);
        };
        newEmptyWindow.Closing += (o, e) =>
        {
            
        };
        _windows.Add(model);
        return model;
    }
    internal override async Task<TabbedWindowModel> CreateNewWindowImplementAsync()
        => await CreateNewWindowAsync();
    internal override async Task<TabbedWindowModel> CreateNewWindowImplementAsync(object tabModel)
        => await CreateNewWindowAsync((T)tabModel);
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
[DependencyProperty<DataTemplate>("HeaderTemplate")]
[DependencyProperty<DataTemplate>("WindowTemplate")]
[DependencyProperty<DataTemplate>("ItemTemplate")]
[DependencyProperty<DataTemplate>("ContentTemplate")]
[DependencyProperty<DataTemplate>("ToolbarTemplate")]
[DependencyProperty<MotionDragConnectionContext>("ConnectionContext", GenerateLocalOnPropertyChangedMethod = true)]
public abstract partial class TabbedApplicationManager : DependencyObject
{
    internal TabbedApplicationManager(IReadOnlyList<TabbedWindowModel> windows) {
        Windows = windows;
        ConnectionContext = new();
        WindowTemplate = DefaultTemplates.Singleton.TabWindowTemplate;
        ItemTemplate = DefaultTemplates.Singleton.TabItemTemplate;
        ContentTemplate = DefaultTemplates.Singleton.TabContentTemplate;
        ToolbarTemplate = HeaderTemplate = DefaultTemplates.Singleton.EmptyTemplate;
    }
    public IReadOnlyList<TabbedWindowModel> Windows { get; }
    public Task<TabbedWindowModel> CreateNewWindow() => CreateNewWindowImplementAsync();
    internal abstract Task<TabbedWindowModel> CreateNewWindowImplementAsync();
    internal abstract Task<TabbedWindowModel> CreateNewWindowImplementAsync(object tabModel);
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

    private async void ConnectionContext_DroppedOutside(object? sender, object? item, DragPosition dragPosition, DropManager dropManager)
    {
        dropManager.ShouldItemBeRemovedFromHost = true;
        if (item is not null)
        {
            var def = dropManager.GetDeferral();
            var model = await CreateNewWindowImplementAsync(item);
            var mousePos = dragPosition.MousePositionToScreen;
            if (sender is UIElement ele)
                model.Window.Bounds =
                        Windowing.Window.GetFromXamlRoot(ele.XamlRoot).Bounds with
                        { X = mousePos.X, Y = mousePos.Y };
            else
                model.Window.Bounds =
                    model.Window.Bounds with { X = mousePos.X, Y = mousePos.Y };
            model.Window.Activate();
#if WINDOWS_UWP
            // fix the bug where when we drop the new window does not appear
            await Task.Delay(100);
#endif
            def.Complete();
        }
    }
    internal abstract void InternalAddTab(TabbedWindowModel model, object? parameter);
}