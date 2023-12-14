namespace Get.UI;
public class ControlsResources : ResourceDictionary
{
    public ControlsResources()
    {
        Source = new Uri($"ms-appx:///{typeof(ControlsResources).Assembly.GetName().Name}/{typeof(ControlsResources).Name}.xaml");
    }
}