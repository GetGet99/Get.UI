using System.Threading.Tasks;
namespace Get.UI.MotionDrag;
public class DropManagerDeferral
{
    readonly TaskCompletionSource TaskCompletionSource = new();
    internal DropManagerDeferral() { }
    internal Task WaitAsync() => TaskCompletionSource.Task;
    public void Complete() => TaskCompletionSource.SetResult();
    public static Task WaitAsync(DropManagerDeferral deferral) => deferral.WaitAsync();
}