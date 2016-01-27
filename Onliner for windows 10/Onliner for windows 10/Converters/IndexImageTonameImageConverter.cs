using Onliner_for_windows_10.ImageCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onliner_for_windows_10
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
