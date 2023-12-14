using Get.UI.Controls.Internals;
using Microsoft.UI.Xaml.Controls;

namespace Get.UI.Controls;
partial class HeaderFooterContent
{
    T GetTemplateChild<T>(string name) where T : DependencyObject => (T)GetTemplateChild(name);
    FrameworkElement? HeaderAndInset => GetTemplateChild<FrameworkElement>(nameof(HeaderAndInset));
    FrameworkElement? ContentAndInline => GetTemplateChild<FrameworkElement>(nameof(ContentAndInline));
    FrameworkElement? FooterAndInset => GetTemplateChild<FrameworkElement>(nameof(FooterAndInset));
    CustomizablePanel? PanelHost => GetTemplateChild<CustomizablePanel>(nameof(PanelHost));
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PanelHost!.OnMeasure += OnPanelMeasure;
        PanelHost!.OnArrange += OnPanelArrange;
    }
    private Size OnPanelMeasure(Size availableSize)
    {
        var orientation = Orientation;
        (double Along, double Opposite) SizeToOF(Size size) =>
            orientation is Orientation.Horizontal ?
            (size.Width, size.Height) : (size.Height, size.Width);
        Size OFToSize((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        double maxOpp = 0;
        var avalSize = SizeToOF(availableSize);
        if (HeaderAndInset is not { } header) return default;
        header.Measure(availableSize);
        var headerSize = SizeToOF(header.DesiredSize);
        if (headerSize.Opposite > maxOpp) maxOpp = headerSize.Opposite;

        if (FooterAndInset is not { } footer) return default;
        footer.Measure(OFToSize((avalSize.Along - headerSize.Along, avalSize.Opposite)));
        var footerSize = SizeToOF(footer.DesiredSize);
        if (footerSize.Opposite > maxOpp) maxOpp = footerSize.Opposite;
        
        if (ContentAndInline is not { } content) return default;
        if (CenterAlignmentResolvingMode is CenterAlignmentResolvingMode.AbsoluteCenterResizeDown)
            content.Measure(
                OFToSize((Math.Min(
                    avalSize.Along - Math.Max(headerSize.Along, footerSize.Along) * 2,
                    ContentRequestedSize ?? avalSize.Along
                ), avalSize.Opposite))
            );
        else
            content.Measure(
                OFToSize((Math.Min(
                    avalSize.Along - headerSize.Along - footerSize.Along,
                    ContentRequestedSize ?? avalSize.Along
                ), avalSize.Opposite))
            );
        var contentSize = SizeToOF(footer.DesiredSize);
        if (ContentRequestedSize is { } n && n > contentSize.Along) contentSize.Along = n;
        if (contentSize.Opposite > maxOpp) maxOpp = contentSize.Opposite;
        return OFToSize((avalSize.Along, maxOpp));
    }

    private Size OnPanelArrange(Size finalSize)
    {
        if (HeaderAndInset is not { } header) return finalSize;
        if (ContentAndInline is not { } content) return finalSize;
        if (FooterAndInset is not { } footer) return finalSize;
        if (PanelHost is not { } panel) return finalSize;
        var orientation = Orientation;
        (double Along, double Opposite) SizeToOF(Size size) =>
            orientation is Orientation.Horizontal ?
            (size.Width, size.Height) : (size.Height, size.Width);
        Size OFToSize((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        Point OFToPoint((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        var panelSize = SizeToOF(finalSize);
        var headerSize = SizeToOF(header.DesiredSize);
        var contentSize = SizeToOF(content.DesiredSize);
        if (ContentRequestedSize is { } n) contentSize.Along = n;
        var footerSize = SizeToOF(footer.DesiredSize);
        header.Arrange(new(
            default,
            OFToSize((headerSize.Along, panelSize.Opposite))
        ));
        footer.Arrange(new(
            OFToPoint((panelSize.Along - footerSize.Along, 0)),
            OFToSize((footerSize.Along, panelSize.Opposite))
        ));
        var alignment = Alignment;
        if (alignment is OrientationNeutralAlignment.Center)
        {
            var remainingSize = panelSize.Along - headerSize.Along - footerSize.Along;
            var remainingSizeToBeCentered = panelSize.Along - Math.Max(headerSize.Along, footerSize.Along) * 2;
            remainingSize = Math.Max(remainingSize, 0);
            if (contentSize.Along <= remainingSizeToBeCentered)
            {
                // center the element
                //content.Measure(OFToSize((contentSize.Along, panelSize.Opposite)));
                Rect rect = new(
                    OFToPoint(((panelSize.Along - contentSize.Along) / 2, 0)),
                    OFToSize((contentSize.Along, panelSize.Opposite))
                );
                content.Arrange(rect);
                return finalSize;
            }
            else if (CenterAlignmentResolvingMode is CenterAlignmentResolvingMode.AbsoluteCenterResizeDown)
            {
                remainingSizeToBeCentered = Math.Max(0, remainingSizeToBeCentered);
                // center the element but with remaining size
                content.Arrange(new(
                    OFToPoint(((panelSize.Along - remainingSizeToBeCentered) / 2, 0)),
                    OFToSize((remainingSizeToBeCentered, panelSize.Opposite))
                ));
                return finalSize;
            }
            else if (contentSize.Along >= remainingSize)
            {
                // too small and it will stretch out to fill the remaining space
                content.Arrange(new(
                    OFToPoint((headerSize.Along, 0)),
                    OFToSize((remainingSize, panelSize.Opposite))
                ));
                return finalSize;
            }
            else if (headerSize.Along >= footerSize.Along)
            {
                // a lot more things on the header, put it after the header
                content.Arrange(new(
                    OFToPoint((headerSize.Along, 0)),
                    OFToSize((contentSize.Along, panelSize.Opposite))
                ));
                return finalSize;
            }
            else
            {
                // a lot more things on the footer, put it before the footer
                content.Arrange(new(
                    OFToPoint((panelSize.Along - footerSize.Along - contentSize.Along, 0)),
                    OFToSize((contentSize.Along, panelSize.Opposite))
                ));
                return finalSize;
            }
        }
        else if (alignment is OrientationNeutralAlignment.Start)
        {
            content.Arrange(new(
                OFToPoint((headerSize.Along, 0)),
                OFToSize((contentSize.Along, panelSize.Opposite))
            ));
            return finalSize;
        }
        else if (alignment is OrientationNeutralAlignment.End)
        {
            content.Arrange(new(
                OFToPoint((panelSize.Along - footerSize.Along - contentSize.Along, 0)),
                OFToSize((contentSize.Along, panelSize.Opposite))
            ));
            return finalSize;
        }
        else throw new InvalidOperationException();
    }

    [Event<SizeChangedEventHandler>]
    void AdjustUI()
    {
        PanelHost?.InvalidateArrange();
    }
}