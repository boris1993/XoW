using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace XoW.Views
{
    public sealed partial class LargeImageViewUserControl : UserControl
    {
        public LargeImageViewUserControl()
        {
            InitializeComponent();
            GlobalState.LargeImageViewObjectReference = this;

            LargeImage.ImageFailed += (_, _) =>
            {
                ProgressRingImageLoad.Visibility = Visibility.Collapsed;
                TextBlockImageLoadFailed.Visibility = Visibility.Visible;
            };
        }

        private async void OnSaveImageButtonClicked(object sender, RoutedEventArgs args)
        {
            ButtonSaveImage.IsEnabled = false;

            var imageUri = ((BitmapImage)LargeImage.Source).UriSource;
            var imageFile = await StorageFile.CreateStreamedFileFromUriAsync(Path.GetFileName(imageUri.AbsoluteUri), imageUri, null);
            var imageFileName = imageFile.Name;

            var targetFolderPath = await KnownFolders.PicturesLibrary.CreateFolderAsync(Constants.ForumName, CreationCollisionOption.OpenIfExists);
            await imageFile.CopyAsync(targetFolderPath, imageFileName, NameCollisionOption.ReplaceExisting);

            ButtonSaveImage.IsEnabled = true;
            await new NotificationContentDialog(false, $"{ComponentContent.ImageSavedToLocation} {targetFolderPath.Path}").ShowAsync();
        }

        public void ResetState()
        {
            ProgressRingImageLoad.Visibility = Visibility.Visible;
            TextBlockImageLoadFailed.Visibility = Visibility.Collapsed;
        }
    }
}
