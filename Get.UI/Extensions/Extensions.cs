namespace Get.UI.Extensions;

static class PointExtension
{
    internal static Point Add(this Point pt1, Point pt2) => new(pt1.X + pt2.X, pt1.Y + pt2.Y);
    internal static Point Subtract(this Point pt1, Point pt2) => new(pt1.X - pt2.X, pt1.Y - pt2.Y);
    internal static Vector3 AsVector3(this Point pt, float z = 0) => new((float)pt.X, (float)pt.Y, z);
    internal static Point GetLocation(this Rect rect) => new(rect.X, rect.Y);
    internal static Rect WithLocation(this Rect rect, Point pt) => new(pt.X, pt.Y, rect.Width, rect.Height);
}