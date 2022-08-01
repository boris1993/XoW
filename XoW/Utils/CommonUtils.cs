using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace XoW.Utils
{
    public static class CommonUtils
    {
        public static async Task<StorageFile> OpenFilePickerForSingleImageAsync()
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");

            var storageFile = await filePicker.PickSingleFileAsync();

            if (storageFile == null)
            {
                return null;
            }

            var fileMimeType = storageFile.ContentType;
            if (!fileMimeType.StartsWith("image/"))
            {
                throw new AppException(ErrorMessage.FileIsNotImage);
            }

            return storageFile;
        }
    }
}
