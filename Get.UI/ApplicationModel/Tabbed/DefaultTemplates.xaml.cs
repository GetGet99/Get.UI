namespace Get.UI.ApplicationModel.Tabbed;

internal sealed partial class DefaultTemplates : Page
{
	public static DefaultTemplates Singleton { get; } = new();
	private DefaultTemplates()
	{
		this.InitializeComponent();
	}
}
