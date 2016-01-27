using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Onliner_for_windows_10.Converters
{
    public class UpdateLogoImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage image = (BitmapImage)value;
            var bitmapImage = new BitmapImage(new Uri(image.UriSource.ToString())) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            return bitmapImage;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
