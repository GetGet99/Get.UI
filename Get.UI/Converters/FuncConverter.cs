namespace Get.UI.Converters;

class FuncConverter<TFrom, TTo> : Converter<TFrom, TTo>
{
    readonly Func<TFrom, TTo> ConvertFunc;
    readonly Func<TTo, TFrom> ConvertBackFunc;
    public FuncConverter(Func<TFrom, TTo> convert, Func<TTo, TFrom> convertBack)
    {
        ConvertFunc = convert;
        ConvertBackFunc = convertBack;
    }
    public FuncConverter(Func<TFrom, TTo> convert) : this(convert, x => throw new NotImplementedException()) { }
    public override TTo Convert(TFrom from) => ConvertFunc(from);
    public override TFrom ConvertBack(TTo to) => ConvertBackFunc(to);
}