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

            InitializeStaticResources();

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
            await RefreshThreads();

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
            HideNewReplyPanel();

            if (args.InvokedItemContainer.Name == Constants.FavouriteThreadNavigationItemName)
            {
                ShowContentGrid();

                if (string.IsNullOrEmpty(GlobalState.ObservableObject.SubscriptionId))
                {
                    await new NotificationContentDialog(true, ErrorMessage.SubscriptionIdRequiredForGettingSubscription).ShowAsync();
                }

                await RefreshSubscriptions();
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
                await new NotificationContentDialog(true, ErrorMessage.CookieRequiredForThisForum).ShowAsync();
                return;
            }

            #endregion

            GlobalState.CurrentForumId = selectedForumId;
            ShowContentGrid();
            await RefreshThreads();
        }

        private async void OnThreadClicked(object sender, ItemClickEventArgs args)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            if (((Grid)args.ClickedItem).DataContext is not ThreadDataContext dataContext)
            {
                throw new AppException("串的DataContext为null");
            }

            GlobalState.CurrentThreadId = dataContext.ThreadId;
            await RefreshReplies();
            ResetAndShowRepliesPanel();

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void OnRefreshThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            if (((NavigationViewItem)ForumListNavigation.SelectedItem).Name == Constants.FavouriteThreadNavigationItemName)
            {
                await RefreshSubscriptions();
                return;
            }

            await RefreshThreads();
        }

        private void OnCreateNewThreadButtonClicked(object sender, RoutedEventArgs args) => ShowNewThreadPanel();

        private void OnCloseNewThreadPanelButtonClicked(object sender, RoutedEventArgs args) => HideNewThreadPanel();

        private async void OnRefreshRepliesButtonClicked(object sender, RoutedEventArgs args) => await RefreshReplies();

        private void OnCreateReplyButtonClicked(object sender, RoutedEventArgs args) => ShowNewReplyPanel();

        private void OnCloseNewReplyPanelButtonClicked(object sender, RoutedEventArgs args) => HideNewReplyPanel();

        private async void OnPoOnlyButtonClicked(object sender, RoutedEventArgs args) => await RefreshPoOnlyReplies();

        private async void OnAddSubscriptionButtonClicked(object sender, RoutedEventArgs args)
        {
            var subscriptionId = ApplicationConfigurationHelper.GetSubscriptionId();
            var threadId = GlobalState.CurrentThreadId;

            var result = await AnoBbsApiClient.AddSubscriptionAsync(subscriptionId, threadId);


            await new NotificationContentDialog(false, result).ShowAsync();
        }

        private async void OnDeleteSubscriptionButtonClicked(object sender, RoutedEventArgs args)
        {
            var subscriptionId = ApplicationConfigurationHelper.GetSubscriptionId();
            var threadId = ((Button)sender).DataContext.ToString();

            var result = await AnoBbsApiClient.DeleteSubscriptionAsync(subscriptionId, threadId);

            await new NotificationContentDialog(false, result).ShowAsync();

            await RefreshSubscriptions();
        }

        private async void OnNewThreadAttachPictureButtonClicked(object sender, RoutedEventArgs args)
        {
            var storageFile = await CommonUtils.OpenFilePickerForSingleImageAsync();
            if (storageFile == null)
            {
                return;
            }

            ButtonNewThreadAttachPicture.DataContext = storageFile;

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

        private async void OnNewReplyAttachPictureButtonClicked(object sender, RoutedEventArgs args)
        {
            var storageFile = await CommonUtils.OpenFilePickerForSingleImageAsync();
            if (storageFile == null)
            {
                return;
            }

            ButtonNewReplyAttachPicture.DataContext = storageFile;

            var thumbnail =
                await storageFile.GetThumbnailAsync(
                    Windows.Storage.FileProperties.ThumbnailMode.PicturesView,
                    200,
                    Windows.Storage.FileProperties.ThumbnailOptions.UseCurrentScale);

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(thumbnail.CloneStream());
            ReplyImageNewThreadPreview.Source = bitmapImage;
            ReplyImagePreviewStackPanel.Visibility = Visibility.Visible;
        }

        public void OnRemoveNewThreadPictureButtonClicked(object sender, RoutedEventArgs args)
        {
            ButtonNewThreadAttachPicture.DataContext = null;
            ImagePreviewStackPanel.Visibility = Visibility.Collapsed;
        }

        public void OnRemoveNewReplyPictureButtonClicked(object sender, RoutedEventArgs args)
        {
            ButtonNewReplyAttachPicture.DataContext = null;
            ReplyImagePreviewStackPanel.Visibility = Visibility.Collapsed;
        }

        private async void OnSendNewThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(TextBoxNewThreadContent.Text) && ButtonNewThreadAttachPicture.DataContext == null)
            {
                await new NotificationContentDialog(true, ErrorMessage.ContentRequiredWhenNoImageAttached).ShowAsync();
                return;
            }

            var fid = ((KeyValuePair<string, (string forumId, string permissionLevel)>)ForumSelectionComboBox.SelectedItem).Value.forumId;
            var selectedCookie = (AnoBbsCookie)NewThreadCookieSelectionComboBox.SelectedItem;
            var username = TextBoxNewThreadUserName.Text;
            var email = TextBoxNewThreadEmail.Text;
            var title = TextBoxNewThreadTitle.Text;
            var content = TextBoxNewThreadContent.Text;
            var image = ButtonNewThreadAttachPicture.DataContext as StorageFile;
            var shouldApplyWatermark = (CheckBoxNewThreadWaterMark.IsChecked ?? false) ? "1" : "0";

            DisableSendButtonAndShowProgressBar(ButtonSendNewThread);

            await AnoBbsApiClient.CreateNewThread(
                fid,
                username,
                email,
                title,
                content,
                shouldApplyWatermark,
                selectedCookie,
                image);

            await new NotificationContentDialog(false, ComponentContent.NewThreadCreatedSuccessfully).ShowAsync();

            EnableSendButtonAndHideProgressBar(ButtonSendNewThread);
            HideNewThreadPanel();
            ResetNewThreadPanel();
        }

        private async void OnSendNewReplyButtonClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(TextBoxNewReplyContent.Text) && ButtonNewReplyAttachPicture.DataContext == null)
            {
                await new NotificationContentDialog(true, ErrorMessage.ContentRequiredWhenNoImageAttached).ShowAsync();
                return;
            }

            var resto = GlobalState.ObservableObject.ThreadId;
            var selectedCookie = (AnoBbsCookie)NewThreadCookieSelectionComboBox.SelectedItem;
            var username = TextBoxNewReplyUserName.Text;
            var email = TextBoxNewReplyEmail.Text;
            var title = TextBoxNewReplyTitle.Text;
            var content = TextBoxNewReplyContent.Text;
            var image = ButtonNewReplyAttachPicture.DataContext as StorageFile;
            var shouldApplyWatermark = (CheckBoxNewReplyWaterMark.IsChecked ?? false) ? "1" : "0";

            DisableSendButtonAndShowProgressBar(ButtonSendNewReply);

            await AnoBbsApiClient.CreateNewReply(
                resto,
                username,
                email,
                title,
                content,
                shouldApplyWatermark,
                selectedCookie,
                image);

            await new NotificationContentDialog(false, ComponentContent.NewReplyCreatedSuccessfully).ShowAsync();

            EnableSendButtonAndHideProgressBar(ButtonSendNewReply);
            HideNewReplyPanel();

            await RefreshReplies();
        }

        private void OnNewThreadSelectEmoticonButtonClicked(object sender, RoutedEventArgs args)
        {
            NewThreadEmoticonWrapPanel.Visibility =
                NewThreadEmoticonWrapPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnNewReplySelectEmoticonButtonClicked(object sender, RoutedEventArgs args)
        {
            NewReplyEmoticonWrapPanel.Visibility =
                NewReplyEmoticonWrapPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnNewThreadEmoticonButtonClicked(object sender, RoutedEventArgs args)
        {
            var button = sender as Button;
            var emoticonValue = button.DataContext as string;

            TextBoxNewThreadContent.Text += emoticonValue;
        }

        private void OnNewReplyEmoticonButtonClicked(object sender, RoutedEventArgs args)
        {
            var button = sender as Button;
            var emoticonValue = button.DataContext as string;

            TextBoxNewReplyContent.Text += emoticonValue;
        }

        private async void OnReportThreadButtonClicked(object sender, RoutedEventArgs args) =>
            await new ReportThreadContentDialog().ShowAsync();

        private async void OnGotoThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            await new JumpToThreadContentDialog(async (contetnDialogSender, eventArgs) =>
            {
                var textBox = contetnDialogSender.FindName("TextBoxTargetThreadId") as TextBox;
                var targetThreadId = textBox.Text.Trim();

                GlobalState.CurrentThreadId = targetThreadId;
                await RefreshReplies();
                ResetAndShowRepliesPanel();
            }).ShowAsync();
        }
    }
}
