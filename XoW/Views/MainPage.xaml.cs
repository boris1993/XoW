using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XoW.Models;
using XoW.Services;
using XoW.Utils;

// ReSharper disable MemberCanBeMadeStatic.Local
namespace XoW.Views
{
    /// <summary>
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private readonly ObservableCollection<NavigationViewItemBase> _navigationItems = new ObservableCollection<NavigationViewItemBase>();

        private readonly List<string> _nonForumNavigationItems = new List<string>
        {
            Constants.FavouriteThreadNavigationItemName
        };

        private Grid rightClickedGrid;

        public MainPage()
        {
            GlobalState.MainPageObjectReference = this;

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
            ForumSelectionComboBox.ItemsSource = GlobalState.ForumAndIdLookup.Where(item => item.Value.forumId != Constants.TimelineForumId).ToDictionary(item => item.Key, item => item.Value);
            ForumSelectionComboBox.SelectedIndex = 0;

            // 载入时间线第一页
            await RefreshThreads();

            // 载入爱发电赞助名单
            var aiFaDianSponsors = await AiFaDianApiClient.GetSponsorList();
            aiFaDianSponsors.List.ForEach(sponsor =>
            {
                GlobalState.AiFaDianSponsoredUsers.Add(sponsor.User);
            });

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void NavigationItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            HideLargeImageView();
            await HideNewThreadPanel();
            await HideNewReplyPanel();

            if (args.IsSettingsInvoked)
            {
                ShowSettingsGrid();
                return;
            }

            if (args.InvokedItemContainer.Name == Constants.FavouriteThreadNavigationItemName)
            {
                ShowContentGrid();

                await RefreshSubscriptions();
                GlobalState.ObservableObject.ForumName = Constants.FavouriteThreadNavigationItemName;

                return;
            }

            #region 检查将要访问的版是否要求持有饼干
            var selectedForumId = args.InvokedItemContainer.DataContext.ToString();
            var currentForumPermissionLevel = GlobalState.ForumAndIdLookup.Values.Single(value => value.forumId.ToString() == selectedForumId).permissionLevel;
            if (string.IsNullOrEmpty(GlobalState.ObservableObject.CurrentCookie) && currentForumPermissionLevel == Constants.PermissionLevelCookieRequired)
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

            GlobalState.isPoOnly = false;
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

        private async void OnCreateNewThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(GlobalState.ObservableObject.CurrentCookie))
            {
                await new NotificationContentDialog(true, ErrorMessage.CookieRequiredForCreatingThread).ShowAsync();
                return;
            }

            ShowNewThreadPanel();
        }

        private async void OnCloseNewThreadPanelButtonClicked(object sender, RoutedEventArgs args) => await HideNewThreadPanel();

        private async void OnRefreshRepliesButtonClicked(object sender, RoutedEventArgs args) => await RefreshReplies();

        private async void OnCreateReplyButtonClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(GlobalState.ObservableObject.CurrentCookie))
            {
                await new NotificationContentDialog(true, ErrorMessage.CookieRequiredForCreatingThread).ShowAsync();
                return;
            }

            ShowNewReplyPanel();
        }

        private async void OnCloseNewReplyPanelButtonClicked(object sender, RoutedEventArgs args) => await HideNewReplyPanel();

        private async void OnPoOnlyButtonClicked(object sender, RoutedEventArgs args)
        {
            GlobalState.isPoOnly = true;
            await RefreshPoOnlyReplies();
        }

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

            var thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.PicturesView, 200, ThumbnailOptions.UseCurrentScale);

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

            var thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.PicturesView, 200, ThumbnailOptions.UseCurrentScale);

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

            var fid = ((KeyValuePair<string, (string forumId, string permissionLevel)>)ForumSelectionComboBox.SelectedItem!).Value.forumId;
            var selectedCookie = (AnoBbsCookie)NewThreadCookieSelectionComboBox.SelectedItem;
            var username = TextBoxNewThreadUserName.Text;
            var email = TextBoxNewThreadEmail.Text;
            var title = TextBoxNewThreadTitle.Text;
            var content = TextBoxNewThreadContent.Text;
            var image = ButtonNewThreadAttachPicture.DataContext as StorageFile;
            var shouldApplyWatermark = CheckBoxNewThreadWaterMark.IsChecked ?? false
                ? "1"
                : "0";

            DisableSendButtonAndShowProgressBar(ButtonSendNewThread);

            await AnoBbsApiClient.CreateNewThread(fid,
                username,
                email,
                title,
                content,
                shouldApplyWatermark,
                selectedCookie,
                image);

            await new NotificationContentDialog(false, ComponentContent.NewThreadCreatedSuccessfully).ShowAsync();

            EnableSendButtonAndHideProgressBar(ButtonSendNewThread);
            await HideNewThreadPanel(true);
            ResetNewThreadPanel();

            await RefreshThreads();
        }

        private async void OnSendNewReplyButtonClicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(TextBoxNewReplyContent.Text) && ButtonNewReplyAttachPicture.DataContext == null)
            {
                await new NotificationContentDialog(true, ErrorMessage.ContentRequiredWhenNoImageAttached).ShowAsync();
                return;
            }

            var resto = GlobalState.ObservableObject.ThreadId;
            var selectedCookie = (AnoBbsCookie)NewReplyCookieSelectionComboBox.SelectedItem;
            var username = TextBoxNewReplyUserName.Text;
            var email = TextBoxNewReplyEmail.Text;
            var title = TextBoxNewReplyTitle.Text;
            var content = TextBoxNewReplyContent.Text;
            var image = ButtonNewReplyAttachPicture.DataContext as StorageFile;
            var shouldApplyWatermark = CheckBoxNewReplyWaterMark.IsChecked ?? false
                ? "1"
                : "0";

            DisableSendButtonAndShowProgressBar(ButtonSendNewReply);

            await AnoBbsApiClient.CreateNewReply(resto,
                username,
                email,
                title,
                content,
                shouldApplyWatermark,
                selectedCookie,
                image);

            await new NotificationContentDialog(false, ComponentContent.NewReplyCreatedSuccessfully).ShowAsync();

            EnableSendButtonAndHideProgressBar(ButtonSendNewReply);
            await HideNewReplyPanel(true);
            ResetNewReplyPanel();

            await RefreshReplies();
        }

        private void OnNewThreadSelectEmoticonButtonClicked(object sender, RoutedEventArgs args) =>
            NewThreadEmoticonWrapPanel.Visibility = NewThreadEmoticonWrapPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;

        private void OnNewReplySelectEmoticonButtonClicked(object sender, RoutedEventArgs args) =>
            NewReplyEmoticonWrapPanel.Visibility = NewReplyEmoticonWrapPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;

        private void OnNewThreadEmoticonButtonClicked(object sender, RoutedEventArgs args)
        {
            var button = sender as Button;
            var emoticonValue = button!.DataContext as string;

            TextBoxNewThreadContent.Text += emoticonValue!;
        }

        private void OnNewReplyEmoticonButtonClicked(object sender, RoutedEventArgs args)
        {
            var button = sender as Button;
            var emoticonValue = button!.DataContext as string;

            TextBoxNewReplyContent.Text += emoticonValue!;
        }

        private async void OnReportThreadButtonClicked(object sender, RoutedEventArgs args) => await new ReportThreadContentDialog(GlobalState.ObservableObject.ThreadId).ShowAsync();

        private async void OnGotoThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            async void PrimaryButtonEventHandler(ContentDialog contentDialogSender, ContentDialogButtonClickEventArgs _)
            {
                var textBox = contentDialogSender.FindName("TextBoxInput") as TextBox;
                var targetThreadId = textBox!.Text.Trim();

                GlobalState.CurrentThreadId = targetThreadId;
                await RefreshReplies();
                ResetAndShowRepliesPanel();
            }

            await new ContentDialogWithInput(ComponentContent.GotoThread, ComponentContent.GotoThread, PrimaryButtonEventHandler).ShowAsync();
        }

        private async void OnSearchThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            async void PrimaryButtonClickEventHandler(ContentDialog contentDialogSender, ContentDialogButtonClickEventArgs _)
            {
                var textBox = contentDialogSender.FindName("TextBoxInput") as TextBox;
                var searchKeyword = textBox!.Text.Trim();

                await AnoBbsApiClient.SearchThread(searchKeyword);
            }

            await new ContentDialogWithInput(ComponentContent.SearchThread, ComponentContent.SearchThread, PrimaryButtonClickEventHandler).ShowAsync();
        }

        public void OnImageClicked(object sender, TappedRoutedEventArgs args)
        {
            var fullSizeImage = ((Image)sender).DataContext as BitmapImage;
            LargeImageView.DataContext = fullSizeImage;

            ContentGrid.Visibility = Visibility.Collapsed;
            LargeImageView.Visibility = Visibility.Visible;
        }

        public void OnLargeImageViewCloseButtonClicked(object sender, RoutedEventArgs args)
        {
            LargeImageView.DataContext = null;

            ContentGrid.Visibility = Visibility.Visible;
            LargeImageView.Visibility = Visibility.Collapsed;

            GlobalState.LargeImageViewObjectReference.ResetState();
        }

        private void OnReplyListViewRightClicked(object sender, RightTappedRoutedEventArgs args)
        {
            var listView = sender as ListView;
            ReplyListViewItemFlyout.ShowAt(listView, args.GetPosition(listView));

            if (args.OriginalSource is ListViewItemPresenter)
            {
                var listViewItemPresenter = args.OriginalSource as ListViewItemPresenter;
                var content = listViewItemPresenter.Content as Grid;

                rightClickedGrid = content;
            }
            else if (args.OriginalSource is Image)
            {
                var image = args.OriginalSource as Image;
                var imageParentGrid = image.Parent as Grid;
                var gridParentStackPanel = imageParentGrid.Parent as StackPanel;
                var stackPanelParentGrid = gridParentStackPanel.Parent as Grid;

                rightClickedGrid = stackPanelParentGrid;
            }
            else if (args.OriginalSource is TextBlock)
            {
                var textBlock = args.OriginalSource as TextBlock;
                var textBlockParentStackPanel = textBlock.Parent as StackPanel;
                var stackPanelParentGrid = textBlockParentStackPanel.Parent as Grid;

                rightClickedGrid = stackPanelParentGrid;
            }
            else
            {
                throw new AppException(args.OriginalSource.GetType().Name);
            }
        }

        private void OnReplyThreadMenuFlyoutClicked(object sender, RoutedEventArgs args)
        {
            var threadDataContext = rightClickedGrid.DataContext as ThreadDataContext;
            TextBoxNewReplyContent.Text = $">>{threadDataContext.ThreadId}\n";
            ShowNewReplyPanel();
        }

        private void OnCopyContentMenuFlyoutClicked(object sender, RoutedEventArgs args)
        {
            var contentStackPanel = rightClickedGrid.Children.Single(item => item is StackPanel) as StackPanel;
            var contentTextBlocks = contentStackPanel.Children.Where(item => item is TextBlock).ToList();

            var content = "";
            contentTextBlocks.ForEach(textBlock => content += $"{((TextBlock)textBlock).Text}\n");

            var dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        private async void OnReportThreadMenuFlyoutClicked(object sender, RoutedEventArgs args)
        {
            var threadDataContext = rightClickedGrid.DataContext as ThreadDataContext;
            await new ReportThreadContentDialog(threadDataContext.ThreadId).ShowAsync();
        }

        private async void OnRepliesGotoPageButtonClicked(object sender, RoutedEventArgs args)
        {
            async void PrimaryButtonEventHandler(ContentDialog contentDialogSender, ContentDialogButtonClickEventArgs _)
            {
                var textBox = contentDialogSender.FindName("TextBoxInput") as TextBox;
                var targetPageNumber = int.Parse(textBox!.Text.Trim());

                if (GlobalState.isPoOnly)
                {
                    await RefreshPoOnlyReplies(targetPageNumber);
                }
                else
                {
                    await RefreshReplies(targetPageNumber);
                }

                ResetAndShowRepliesPanel();
            }

            void TextBoxBeforeTextChangeEventHandler(TextBox textBoxSender, TextBoxBeforeTextChangingEventArgs args) => args.Cancel = args.NewText.Any(c => !char.IsDigit(c));

            await new ContentDialogWithInput(ComponentContent.GotoPagePopupTitle, ComponentContent.Go, PrimaryButtonEventHandler, TextBoxBeforeTextChangeEventHandler).ShowAsync();
        }

        private async void OnLoadMoreRepliesButtonClicked(object sender, RoutedEventArgs args)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            var repliesListViewSource = RepliesListView.ItemsSource as ISupportIncrementalLoading;
            await repliesListViewSource.LoadMoreItemsAsync(default);

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void OnLoadAllRepliesButtonClicked(object sender, RoutedEventArgs args)
        {
            if (GlobalState.ObservableObject.CurrentPageNumber == GlobalState.ObservableObject.TotalPageNumber)
            {
                return;
            }

            async void PrimaryButtonEventHandler(ContentDialog contentDialogSender, ContentDialogButtonClickEventArgs args)
            {
                MainPageProgressBar.Visibility = Visibility.Visible;

                var repliesListViewSource = RepliesListView.ItemsSource as ISupportIncrementalLoading;
                while (GlobalState.ObservableObject.CurrentPageNumber < GlobalState.ObservableObject.TotalPageNumber)
                {
                    await repliesListViewSource.LoadMoreItemsAsync(default);
                }

                MainPageProgressBar.Visibility = Visibility.Collapsed;
            }

            var confirmationDialog = new ConfirmationContentDialog(
                ComponentContent.Notification,
                ConfirmationMessage.LoadAllRepliesConfirmation,
                primaryButtonEventHandler: PrimaryButtonEventHandler);

            await confirmationDialog.ShowAsync();
        }
    }
}
