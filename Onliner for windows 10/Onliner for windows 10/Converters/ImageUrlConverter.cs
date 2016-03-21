using Onliner_for_windows_10.Helper;
using System;
using System.Collections.Generic;
using System.IO;
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
        BitmapImage bitmapImage;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte[] byteArray = value as byte[];
            var img = new BitmapImage();
            CreateBitmap(byteArray);
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static async Task<BitmapImage> GetImageFromStream(IRandomAccessStream stream)
        {
            BitmapImage bmp = new BitmapImage();
            await bmp.SetSourceAsync(stream);
            return bmp;
        }


        private async Task CreateBitmap(byte[] array)
        {
            try
            {
                bitmapImage = new BitmapImage();
                IRandomAccessStream stream = await this.ConvertToRandomAccessStream(array);
                bitmapImage.SetSource(stream);
            }
            catch
            {
             
            }
        }


        private async Task<IRandomAccessStream> ConvertToRandomAccessStream(byte[] bytes)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            using (var writer = new DataWriter(randomAccessStream))
            {
                writer.WriteBytes(bytes);
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();
                writer.Dispose();
            }
            randomAccessStream.Seek(0);

            return randomAccessStream;
        }

    }

}
