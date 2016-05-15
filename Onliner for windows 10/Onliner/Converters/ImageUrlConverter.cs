using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Onliner.Converters
{
    public class ImageUrlConverters : IValueConverter
    {
        private BitmapImage _bitmapImage;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var byteArray = value as byte[];
            CreateBitmap(byteArray);
            return _bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static async Task<BitmapImage> GetImageFromStream(IRandomAccessStream stream)
        {
            var bmp = new BitmapImage();
            await bmp.SetSourceAsync(stream);
            return bmp;
        }


        private async void CreateBitmap(byte[] array)
        {
            try
            {
                _bitmapImage = new BitmapImage();
                var stream = await ConvertToRandomAccessStream(array);
                _bitmapImage.SetSource(stream);
            }
            catch
            {
                // ignored
            }
        }


        private static async Task<IRandomAccessStream> ConvertToRandomAccessStream(byte[] bytes)
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
