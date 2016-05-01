using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

namespace Onliner.Converters
{
    public class ScrubHtmlTag : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Regex.Replace(value.ToString(), @"<[^>]+>|&nbsp;|Дальше…", " ").Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
