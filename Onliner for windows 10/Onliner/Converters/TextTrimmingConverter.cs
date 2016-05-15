using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Onliner.Converters
{
    public class TextTrimmingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var iri = (Uri)value;
            var image = new BitmapImage(iri);
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
