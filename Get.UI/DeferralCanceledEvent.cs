namespace Get.UI;
public class DefferalCanceledEventArgs : EventArgs
{
    public bool Cancel { get; set; }
    internal Deferral? DeferralInternal { get; private set; }
    public Deferral GetDeferral() => DeferralInternal ??= new();
}
public delegate void DefferalCanceledEventHandler(object sender, DefferalCanceledEventArgs args);