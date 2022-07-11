using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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

        public static async Task DecodeBarcodeFromClipboard()
        {
            var barcodeDecoder = GetBarcodeReaderInstance();

            var dataPackage = new DataPackage();
            var bitmapFile = await dataPackage.GetView().GetBitmapAsync();

            var bitmap = (Bitmap)Image.FromStream((await bitmapFile.OpenReadAsync()).AsStreamForRead());
            var lockedBitmap = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var lockedBitmapLength = lockedBitmap.Stride + bitmap.Height * bitmap.Width;
            var bitmapBytes = new byte[lockedBitmapLength];
            Marshal.Copy(lockedBitmap.Scan0, bitmapBytes, 0, lockedBitmapLength);
            bitmap.UnlockBits(lockedBitmap);

            var decodeResult = barcodeDecoder.Decode(bitmapBytes, bitmap.Width, bitmap.Height, RGBLuminanceSource.BitmapFormat.ARGB32);
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
