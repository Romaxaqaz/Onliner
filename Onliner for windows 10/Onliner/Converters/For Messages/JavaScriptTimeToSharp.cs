﻿using System;
using Windows.UI.Xaml.Data;

namespace Onliner.Converters.For_Messages
{
    public class JavaScriptTimeToSharp : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double dob = double.Parse(value.ToString());
            DateTime time = new DateTime(1970, 1, 1).AddSeconds(dob);
            return String.Format("{0:dd MMMM yyyy HH:mm}", time); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
