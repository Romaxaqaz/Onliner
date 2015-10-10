using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
namespace Onliner_for_windows_10
{
    public class TextTrimmingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string bufferString = string.Empty;
            int maxLen = System.Convert.ToInt32(parameter);
            if (value != null)
            {
                string s = value.ToString().Trim();
                if (s.Length > maxLen)
                    bufferString = s.Substring(0, maxLen);
                else
                    bufferString = s;
            }
            return bufferString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
