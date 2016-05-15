using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace OnlinerApp.UserControls
{
    public class HorizontalStretchPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            var width = finalSize.Width / Children.Count;

            foreach(var tab in Children)
            {
                rect.Width = width;
                rect.Height = tab.DesiredSize.Height > finalSize.Height ? tab.DesiredSize.Height : finalSize.Height;
                tab.Arrange(rect);
                rect.X = width + rect.X;
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if(Children.Count == 0)
                return base.MeasureOverride(availableSize);

            var finalSize = new Size { Width = availableSize.Width };
            availableSize.Width = availableSize.Width / Children.Count;

            foreach(var tab in Children)
            {
                tab.Measure(availableSize);
                var desiredSize = tab.DesiredSize;
                finalSize.Height = desiredSize.Height > finalSize.Height ? desiredSize.Height : finalSize.Height;
            }

            if(double.IsPositiveInfinity(finalSize.Height) || double.IsPositiveInfinity(finalSize.Width))
                return Size.Empty;

            return finalSize;
        }
    }
}
