using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XoW.Models;
using XoW.Services;
using XoW.Utils;

namespace XoW.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ObservableCollection<NavigationViewItemBase> _navigationItems =
            new ObservableCollection<NavigationViewItemBase>();

        private readonly List<string> _nonForumNavigationItems =
            new List<string>() { Constants.FavouriteThreadNavigationItemName };

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

            // 因为没有ObservableDictionary
            // 所以只能在版面加载好后，在这里把版面列表绑给发新串页面的版面下拉列表
            ForumSelectionComboBox.ItemsSource =
                GlobalState.ForumAndIdLookup
                    .Where(item => item.Value.forumId != Constants.TimelineForumId)
                    .ToDictionary(item => item.Key, item => item.Value);
            ForumSelectionComboBox.SelectedIndex = 0;

            // 载入时间线第一页
            RefreshThreads();

            // 载入已添加的饼干
            ApplicationConfigurationHelper.LoadAllCookies();

            // 默认以当前选择的饼干发串
            NewThreadCookieSelectionComboBox.SelectedItem =
                GlobalState.Cookies.Single(cookie => cookie.Name == GlobalState.ObservableObject.CurrentCookie);

            // 加载订阅ID
            GlobalState.ObservableObject.SubscriptionId = ApplicationConfigurationHelper.GetSubscriptionId();

            var currentCookieName = ApplicationConfigurationHelper.GetCurrentCookie();
            var currentCookieValue = GlobalState.Cookies.SingleOrDefault(cookie => cookie.Name == currentCookieName)?.Cookie;
            if (!string.IsNullOrEmpty(currentCookieValue))
            {
                HttpClientService.ApplyCookie(currentCookieValue);
            }

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void NavigationItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ShowSettingsGrid();
                return;
            }

            HideNewThreadPanel();

            if (args.InvokedItemContainer.Name == Constants.FavouriteThreadNavigationItemName)
            {
                ShowContentGrid();

                if (string.IsNullOrEmpty(GlobalState.ObservableObject.SubscriptionId))
                {
                    var errorMessagePopup = new ContentDialog
                    {
                        RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                        Title = ComponentContent.Error,
                        CloseButtonText = ComponentContent.Ok,
                        Content = ErrorMessage.SubscriptionIdRequiredForGettingSubscription,
                    };
                    await errorMessagePopup.ShowAsync();
                }

                RefreshSubscriptions();
                GlobalState.ObservableObject.ForumName = Constants.FavouriteThreadNavigationItemName;

                return;
            }

            #region 检查将要访问的版是否要求持有饼干

            var selectedForumId = args.InvokedItemContainer.DataContext.ToString();
            var currentForumPermissionLevel = GlobalState.ForumAndIdLookup.Values
                .Single(value => value.forumId.ToString() == selectedForumId)
                .permissionLevel;
            if (string.IsNullOrEmpty(GlobalState.ObservableObject.CurrentCookie) &&
                currentForumPermissionLevel == Constants.PermissionLevelCookieRequired)
            {
                var errorMessagePopup = new ContentDialog
                {
                    RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                    Title = ComponentContent.Error,
                    CloseButtonText = ComponentContent.Ok,
                    Content = ErrorMessage.CookieRequiredForThisForum,
                };
                await errorMessagePopup.ShowAsync();

                return;
            }

            #endregion

            GlobalState.CurrentForumId = selectedForumId;
            ShowContentGrid();
            RefreshThreads();
        }

        private void OnThreadClicked(object sender, ItemClickEventArgs args)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            if (((Grid)args.ClickedItem).DataContext is not ThreadDataContext dataContext)
            {
                throw new AppException("串的DataContext为null");
            }

            GlobalState.CurrentThreadId = dataContext.ThreadId;
            GlobalState.ObservableObject.ThreadId = dataContext.ThreadId;
            GlobalState.CurrentThreadAuthorUserHash = dataContext.ThreadAuthorUserHash;

            ButtonPoOnly.IsChecked = false;

            RefreshReplies();
            ContentRepliesGrid.Visibility = Visibility.Visible;

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private void OnRefreshThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            if (((NavigationViewItem)ForumListNavigation.SelectedItem).Name == Constants.FavouriteThreadNavigationItemName)
            {
                RefreshSubscriptions();
                return;
            }

            RefreshThreads();
        }

        private void OnCreateNewThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            ShowNewThreadPanel();
        }

        private void OnCloseNewThreadPanelButtonClicked(object sender, RoutedEventArgs args)
        {
            HideNewThreadPanel();
        }

        private void OnRefreshRepliesButtonClicked(object sender, RoutedEventArgs args) => RefreshReplies();

        private void OnPoOnlyButtonClicked(object sender, RoutedEventArgs args) => RefreshPoOnlyReplies();

        private async void OnAddSubscriptionButtonClicked(object sender, RoutedEventArgs args)
        {
            var subscriptionId = ApplicationConfigurationHelper.GetSubscriptionId();
            var threadId = GlobalState.CurrentThreadId;

            var result = await AnoBbsApiClient.AddSubscriptionAsync(subscriptionId, threadId);

            var contentDialog = new ContentDialog
            {
                RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                Title = ComponentContent.Notification,
                Content = result,
                CloseButtonText = ComponentContent.Ok,
            };

            await contentDialog.ShowAsync();
        }

        private async void OnDeleteSubscriptionButtonClicked(object sender, RoutedEventArgs args)
        {
            var subscriptionId = ApplicationConfigurationHelper.GetSubscriptionId();
            var threadId = ((Button)sender).DataContext.ToString();

            var result = await AnoBbsApiClient.DeleteSubscriptionAsync(subscriptionId, threadId);

            var contentDialog = new ContentDialog
            {
                RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                Title = ComponentContent.Notification,
                Content = result,
                CloseButtonText = ComponentContent.Ok,
            };

            await contentDialog.ShowAsync();

            RefreshSubscriptions();
        }

        private async void OnAttachPictureButtonClicked(object sender, RoutedEventArgs args)
        {
            var storageFile = await CommonUtils.OpenFilePickerForSingleImageAsync();

            ButtonAttachPicture.DataContext = storageFile;

            var thumbnail =
                await storageFile.GetThumbnailAsync(
                    Windows.Storage.FileProperties.ThumbnailMode.PicturesView,
                    200,
                    Windows.Storage.FileProperties.ThumbnailOptions.UseCurrentScale);

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(thumbnail.CloneStream());
            ImageNewThreadPreview.Source = bitmapImage;
            ImagePreviewStackPanel.Visibility = Visibility.Visible;
        }

        public void OnRemovePictureButtonClicked(object sender, RoutedEventArgs args)
        {
            ButtonAttachPicture.DataContext = null;
            ImagePreviewStackPanel.Visibility = Visibility.Collapsed;
        }

        private async void OnSendNewThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            var fid = ((KeyValuePair<string, (string forumId, string permissionLevel)>)ForumSelectionComboBox.SelectedItem).Value.forumId;
            var selectedCookie = (AnoBbsCookie)NewThreadCookieSelectionComboBox.SelectedItem;
            var username = TextBoxNewThreadUserName.Text;
            var email = TextBoxNewThreadEmail.Text;
            var title = TextBoxNewThreadTitle.Text;
            var content = TextBoxNewThreadContent.Text;
            var image = ButtonAttachPicture.DataContext as StorageFile;

            await AnoBbsApiClient.CreateNewThread(
                fid,
                username,
                email,
                title,
                content,
                "1",
                selectedCookie,
                image);

            var contentDialog = new ContentDialog
            {
                RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                Title = ComponentContent.Notification,
                Content = ComponentContent.NewThreadCreatedSuccessfully,
                CloseButtonText = ComponentContent.Ok,
            };

            await contentDialog.ShowAsync();

            HideNewThreadPanel();
            ResetNewThreadPanel();
        }
    }
}
