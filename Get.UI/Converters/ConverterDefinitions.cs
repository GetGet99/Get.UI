namespace Get.UI.Converters;

class OrientationFlipConverter : Converter<Orientation, Orientation>
{
    public override Orientation Convert(Orientation from)
        => from is Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
    public override Orientation ConvertBack(Orientation from)
        => from is Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
}