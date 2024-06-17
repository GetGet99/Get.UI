namespace Get.UI.Controls.Panels;
public static class LayoutDebugger
{
    static readonly string newLine = """
        a
        b
        """[1..^1];
    public static string GetDesiredSizeReport(UIElement rootUIElement)
    {
        var rootName = (rootUIElement as FrameworkElement)?.Name;
        return
            $"""
            - ({rootUIElement.DesiredSize.Width}x{rootUIElement.DesiredSize.Height}) {(string.IsNullOrWhiteSpace(rootName) ? rootUIElement.GetType().Name : $"{rootName} ({rootUIElement.GetType().Name})")}
            {string.Join(newLine,
                from child in GetChildren(rootUIElement)
                let childsizereport = $"|   {GetDesiredSizeReport(child).Replace($"{newLine}", $"{newLine}|   ")}"
                select childsizereport
            )}
            """.TrimEnd();
    }
    static IEnumerable<UIElement> GetChildren(UIElement uIElement)
    {
        int count = VisualTreeHelper.GetChildrenCount(uIElement);
        for (int i = 0; i < count; i++)
        {
            yield return (UIElement)VisualTreeHelper.GetChild(uIElement, i);
        }
    }
}