using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XoW.Services;

namespace XoW
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ObservableCollection<NavigationViewItemBase> _navigationItems = new ObservableCollection<NavigationViewItemBase>();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            GlobalState.CdnUrl = await GetCdnUrl();

            // 刷新板块列表，完成后默认选定时间线版
            await RefreshForumsAsync();

            // 载入时间线第一页
            RefreshThreads();

            // 载入已添加的饼干
            ApplicationConfigurationHelper.LoadAllCookies();

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void NavigationItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentGrid.Visibility = Visibility.Collapsed;
                SettingsGrid.Visibility = Visibility.Visible;
                return;
            }

            GlobalState.CurrentForumId = args.InvokedItemContainer.DataContext.ToString();
            ContentGrid.Visibility = Visibility.Visible;
            SettingsGrid.Visibility = Visibility.Collapsed;
            RefreshThreads();
        }

        private async void OnThreadClicked(object sender, ItemClickEventArgs args)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            var threadId = ((Grid)args.ClickedItem)
                .Children
                .Where(element => element is Grid grid && grid.Name == "threadHeaderGrid")
                .Select(grid => grid as Grid)
                .Single()
                .Children
                .Where(element => element is TextBlock block && block.Name == "textBlockThreadId")
                .Select(tb => tb as TextBlock)
                .Single()
                .DataContext
                .ToString();

            GlobalState.CurrentThreadId = threadId;

            await RefreshReplies();
            Replies.Visibility = Visibility.Visible;
            ReplyTopBar.Visibility = Visibility.Visible;

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private void OnRefreshThreadButtonClicked(object sender, RoutedEventArgs args) => RefreshThreads();

        private async void OnRefreshRepliesButtonClicked(object sender, RoutedEventArgs args) => await RefreshReplies();

        private async void OnScanQRCodeButtonClicked(object sender, RoutedEventArgs args)
        {
            var screenSnipResult = await Launcher.LaunchUriAsync(new Uri(Constants.SystemUriStartScreenClip));
            if (!screenSnipResult)
            {
                throw new AppException(ErrorMessage.ScreenSnipFailed);
            }

            await QrCodeService.DecodeBarcodeFromClipboard();
        }

        private async void OnLoadImageButtonClicked(object sender, RoutedEventArgs args)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");

            var storageFile = await filePicker.PickSingleFileAsync();

            var fileMimeType = storageFile.ContentType;
            if (!fileMimeType.StartsWith("image/"))
            {
                throw new AppException(ErrorMessage.FileIsNotImage);
            }

            var cookie = await QrCodeService.DecodeBarcodeFromStorageFileAsync(storageFile);

            if (!GlobalState.Cookies.Contains(cookie))
            {
                GlobalState.Cookies.Add(cookie);
                ApplicationConfigurationHelper.AddCookie(cookie);
            }
        }
    }
}
