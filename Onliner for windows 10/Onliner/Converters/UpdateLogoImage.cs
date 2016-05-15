using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Onliner.Converters
{
    public class UpdateLogoImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var image = (BitmapImage)value;
            var bitmapImage = new BitmapImage(new Uri(image.UriSource.ToString())) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            return bitmapImage;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
