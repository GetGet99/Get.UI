namespace Get.UI.Controls.Containers;

partial class MotionDragItem
{
    T GetTemplateChild<T>(string name) where T : DependencyObject => (T)GetTemplateChild(name);
    Border RootElement => GetTemplateChild<Border>(nameof(RootElement));
    CompositeTransform RootTransform => GetTemplateChild<CompositeTransform>(nameof(RootTransform));
    Storyboard TranslationResetStoryboard => GetTemplateChild<Storyboard>(nameof(TranslationResetStoryboard));
    Storyboard? AnimatingTranslationStoryboard => GetTemplateChild<Storyboard>(nameof(AnimatingTranslationStoryboard));
    DoubleAnimation? AnimatingTranslateX => GetTemplateChild<DoubleAnimation>(nameof(AnimatingTranslateX));
    DoubleAnimation? AnimatingTranslateY => GetTemplateChild<DoubleAnimation>(nameof(AnimatingTranslateY));
}
