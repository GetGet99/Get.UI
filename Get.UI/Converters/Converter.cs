namespace Get.UI.Converters;

abstract class Converter<TFrom, TTo> : IValueConverter
{
    public abstract TTo Convert(TFrom from);
    public virtual TFrom ConvertBack(TTo to)
    {
        throw new NotImplementedException("The overridden class did not override backward conversion");
    }
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        => Convert((TFrom)value);

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        => ConvertBack((TTo)value);
}