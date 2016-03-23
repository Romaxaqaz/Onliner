using Onliner.Image;
using System;
using Windows.UI.Xaml.Data;

namespace Onliner.Converters
{
    public class IndexImageTonameImageConverter : IValueConverter
    {
        private SVGImage svgImage;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            svgImage = new SVGImage();
            foreach (var item in svgImage.svgCollection)
            {
                if(item.Key == value.ToString())
                {
                    return item.Value;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
