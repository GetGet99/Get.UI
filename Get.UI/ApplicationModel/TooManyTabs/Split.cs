using Get.Data.Collections;
using Get.Data.Properties;
namespace Get.UI.ApplicationModel.TooManyTabs;
class TooManyTabsSplitModel<T>
{
    public Property<int> PrimarySelectedIndexTProperty { get; } = new(-1);
    public TwoWayUpdateCollectionProperty<TooManyTabsSplitModel<T>> TabsProperty { get; } = [];
    public Property<Orientation> OrientationProperty { get; } = new(Orientation.Horizontal);
}