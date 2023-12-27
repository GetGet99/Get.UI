namespace Get.UI.StateTriggers;
public partial class ValueStateTrigger<T> : StateTriggerBase
{
    void ValueUpdated()
    {
        SetActive(EqualityComparer<T>.Default.Equals(Value1, Value2));
    }

    public event global::System.EventHandler? Value1Changed;

    /// <summary>
    /// Identifies the Value1 dependency property.
    /// </summary>
    public static global::Microsoft.UI.Xaml.DependencyProperty Value1Property = global::Microsoft.UI.Xaml.DependencyProperty.Register(
        nameof(Value1),
        typeof(T),
        typeof(global::Get.UI.StateTriggers.ValueStateTrigger<T>),
        new global::Microsoft.UI.Xaml.PropertyMetadata(
            default(T),
            static (d, e) => {
                var @this = ((global::Get.UI.StateTriggers.ValueStateTrigger<T>)d);
                @this.ValueUpdated();
                @this.Value1Changed?.Invoke(d, new());
            }
        )
    );

    // No Documentation was provided
    public T Value1
    {
        // No Documentation was provided
        get
        {
            return (T)(GetValue(Value1Property));
        }

        // No Documentation was provided
        set
        {
            if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(Value1, value)) return;
            SetValue(Value1Property, value);
        }
    }

    public event global::System.EventHandler? Value2Changed;

    /// <summary>
    /// Identifies the Value2 dependency property.
    /// </summary>
    public static global::Microsoft.UI.Xaml.DependencyProperty Value2Property = global::Microsoft.UI.Xaml.DependencyProperty.Register(
        nameof(Value2),
        typeof(T),
        typeof(global::Get.UI.StateTriggers.ValueStateTrigger<T>),
        new global::Microsoft.UI.Xaml.PropertyMetadata(
            default(T),
            static (d, e) => {
                var @this = ((global::Get.UI.StateTriggers.ValueStateTrigger<T>)d);
                @this.ValueUpdated();
                @this.Value2Changed?.Invoke(d, new());
            }
        )
    );

    // No Documentation was provided
    public T Value2
    {
        // No Documentation was provided
        get
        {
            return (T)(GetValue(Value2Property));
        }

        // No Documentation was provided
        set
        {
            if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(Value2, value)) return;
            SetValue(Value2Property, value);
        }
    }
}
public class ValueStateTrigger : ValueStateTrigger<object>
{

}