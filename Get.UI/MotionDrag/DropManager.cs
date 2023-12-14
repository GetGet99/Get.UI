using System.Threading.Tasks;
namespace Get.UI.MotionDrag;
public class DropManager
{
    public bool ShouldItemBeRemovedFromHost { get; set; } = false;
    DropManagerDeferral? deferral;
    public DropManagerDeferral GetDeferral() => deferral ??= new();
    internal Task WaitForDeferralAsync()
    {
        if (deferral is not null)
            return deferral.WaitAsync();
        return Task.CompletedTask;
    }
    //public abstract Task PlayDropTranslateAnimationAsync(Point itemOffsset);
}