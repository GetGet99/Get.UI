using Get.UI.Controls.Containers;
using System.Windows.Input;

namespace Get.UI.Controls.Tabs;
//[DependencyProperty<ICommand>("CloseButtonCommand")]
public partial class TabItem : MotionDragSelectableItem
{
    public TabItem()
    {
        DefaultStyleKey = typeof(TabItem);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
}