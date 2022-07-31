using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using XoW.Models;
using ZXing;

namespace XoW.Services
{
    public class QrCodeService
    {
        private static readonly object _lock = new object();
        private static IBarcodeReader _barcodeReader;

        public static IBarcodeReader GetBarcodeReaderInstance()
        {
            if (_barcodeReader == null)
            {
                lock (_lock)
                {
                    _barcodeReader = new BarcodeReader
                    {
                        AutoRotate = true,
                    };
                }
            }

            return _barcodeReader;
        }

        public static async Task<AnonBbsCookie> DecodeBarcodeFromStorageFileAsync(StorageFile file)
        {
            var barcodeDecoder = GetBarcodeReaderInstance();
            using (var stream = await file.OpenReadAsync())
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                var bytes = await GetByteArrayFromStream(stream);
                var decodeResult = barcodeDecoder.Decode(
                    bytes,
                    bitmap.PixelWidth,
                    bitmap.PixelHeight,
                    RGBLuminanceSource.BitmapFormat.Unknown);

                if (decodeResult == null)
                {
                    throw new AppException(ErrorMessage.QrCodeDecodeFailed);
                }

                var content = decodeResult.Text;
                var cookie = JsonConvert.DeserializeObject<AnonBbsCookie>(content);

                return cookie;
            }
        }

        private static async Task<byte[]> GetByteArrayFromStream(IRandomAccessStream stream)
        {
            var bitmapDecoder = await BitmapDecoder.CreateAsync(stream);
            var pixelDataProvider = await bitmapDecoder.GetPixelDataAsync();
            return pixelDataProvider.DetachPixelData();
        }
    }
}
