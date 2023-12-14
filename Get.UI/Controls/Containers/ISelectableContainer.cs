namespace Get.UI.Controls.Containers;
public interface ISelectableContainer
{
    object? PrimarySelectedItem { get; set; }
    int PrimarySelectedIndex { get; set; }
}