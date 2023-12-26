using System.Threading.Tasks;

namespace Get.UI;
public class Deferral
{
    readonly TaskCompletionSource TaskCompletionSource = new();
    internal Deferral() { }
    internal Task WaitAsync() => TaskCompletionSource.Task;
    public void Complete() => TaskCompletionSource.SetResult();
    public static Task WaitAsync(Deferral deferral) => deferral.WaitAsync();
}