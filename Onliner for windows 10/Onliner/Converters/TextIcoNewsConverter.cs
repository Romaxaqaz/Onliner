using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Onliner.Converters
{
    public class TextIcoNewsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = string.Empty;
            var content = value.ToString();
            if (!string.IsNullOrEmpty(content))
            {
                Regex htmlreg = new Regex(@"([0-9]+)");
                result = htmlreg.Match(content).ToString();
            }
            else
            {
                result = " - ";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
