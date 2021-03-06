﻿using Onliner.Image;
using System;
using Windows.UI.Xaml.Data;

namespace Onliner.Converters
{
    public class IndexImageTonameImageConverter : IValueConverter
    {
        private readonly string UrlImage = "http://pogoda.onliner.by/assets/";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0}{1}{2}", UrlImage, value.ToString(), ".png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
