using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

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

        public async static Task<byte[]> ImageToBytes(BitmapImage image)
        {
            RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(image.UriSource);
            IRandomAccessStreamWithContentType streamWithContent = await streamRef.OpenReadAsync();
            byte[] buffer = new byte[streamWithContent.Size];
            await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);
            return buffer;
        }
    }
}
