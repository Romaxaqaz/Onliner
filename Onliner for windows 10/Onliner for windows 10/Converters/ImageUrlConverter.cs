using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onliner_for_windows_10
{
    public class ImageUrlConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string pattern = @"https?://[\w./]+\/[\w./]+\.(bmp|png|jpg|gif|jpeg)";
            string s = value.ToString();
            Regex htmlreg = new Regex("(?<=src=\").*?(?=\")");
            string result = htmlreg.Match(s).ToString();
            if(string.IsNullOrEmpty(result))
            {
                Regex imageUrlPattern = new Regex(pattern);
                result = imageUrlPattern.Match(s).ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
