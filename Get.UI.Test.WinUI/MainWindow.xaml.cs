using Get.UI.Controls.Containers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartTabViewTest.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<string> Strings = new() { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8" };
        ObservableCollection<string> Strings2 = new() { "Item 9", "Item 10", "Item 11", "Item 12", "Item 13", "Item 14", "Item 15", "Item 16" };
        public MainWindow()
        {
            SystemBackdrop = new MicaBackdrop();
            ExtendsContentIntoTitleBar = true;
            this.InitializeComponent();
        }


        public MainWindow(Get.UI.MotionDrag.MotionDragConnectionContext connectionContext) : this()
        {
            ExtendsContentIntoTitleBar = false;
            //Container1.ConnectionContext = Container2.ConnectionContext = connectionContext;
            TabContainer.ConnectionContext = connectionContext;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(TabContainer.ConnectionContext).Activate();
        }
    }
    public class XamlList : List<object>
    {
    }
}
