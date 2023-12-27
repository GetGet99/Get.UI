using Get.EasyCSharp;
using Get.UI.Controls.Containers;
using Get.UI.MotionDrag;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Get.UI.Test.UWP
{
    public partial class TabWrapper : INotifyPropertyChanged
    {
        [AutoNotifyProperty]
        string _Value;

        public event PropertyChangedEventHandler PropertyChanged;

        // me being lazy
        public static implicit operator TabWrapper(string s) => new() { Value = s };
    }
    public sealed partial class MainPage : Page
    {
        ObservableCollection<TabWrapper> Strings = new() {
            "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8"
        };
        ObservableCollection<TabWrapper> Strings2 = new() {
            "Item 9", "Item 10", "Item 11", "Item 12", "Item 13", "Item 14", "Item 15"
        };
        static MainPage FirstWindow;
        public MainPage()
        {
            this.InitializeComponent();
            if (FirstWindow is null)
            {
                FirstWindow = this;
                TabView.ConnectionContext.DroppedOutside += ConnectionContext_DroppedOutside;
            }
        }
        public MainPage(MotionDragConnectionContext connectionContext) : this()
        {
            TabView.ConnectionContext = connectionContext;
            // mica not supported thanks to appwindow
            Background = new SolidColorBrush(Color.FromArgb(255, 32, 32, 32));
        }
        public MainPage(MotionDragConnectionContext connectionContext, TabWrapper tw) : this(connectionContext)
        {
            Strings.Clear();
            Strings.Add(tw);
            TabView.PrimarySelectedItem = tw;
        }
        private async void ConnectionContext_DroppedOutside(object sender, object item, DragPosition dragPosition, DropManager dropManager)
        {
            dropManager.ShouldItemBeRemovedFromHost = true;
            // get mouse pos relative to the first window
            // You can also do it for this window, but the thing is I'm using current view instead of window
            // because I'm too lazy to cache AppWindow for this page for "RequestMoveRelativeToWindowContent"
            var mousePos = dragPosition.ToNewContainer(
                GlobalContainerRect.GetFromXamlRoot(FirstWindow.XamlRoot, default)
            ).ItemPositionToContainer;
            if (item is TabWrapper tw)
            {
                var appWindow = await AppWindow.TryCreateAsync();
                ElementCompositionPreview.SetAppWindowContent(appWindow, new MainPage(TabView.ConnectionContext, tw));
                appWindow.RequestMoveRelativeToCurrentViewContent(mousePos);
                appWindow.RequestSize(
                    new(ActualWidth, ActualHeight)
                );
                //appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                await appWindow.TryShowAsync();
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var appWindow = await AppWindow.TryCreateAsync();
            ElementCompositionPreview.SetAppWindowContent(appWindow, new MainPage(TabView.ConnectionContext));
            //appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            await appWindow.TryShowAsync();
        }
    }
    
}
