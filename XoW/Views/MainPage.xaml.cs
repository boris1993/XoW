using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            new List<string>() {Constants.FavouriteThreadNavigationItemName};

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
            await RefreshThreads();

            // 载入已添加的饼干
            ApplicationConfigurationHelper.LoadAllCookies();

            // 默认以当前选择的饼干发串
            NewThreadCookieSelectionComboBox.SelectedItem =
                GlobalState.Cookies
                    .Where(cookie => cookie.Name == GlobalState.CurrentCookie.CurrentCookie)
                    .Single();

            // 加载订阅ID
            GlobalState.SubscriptionId.SubscriptionId = ApplicationConfigurationHelper.GetSubscriptionId();

            var currentCookieName = ApplicationConfigurationHelper.GetCurrentCookie();
            var currentCookieValue = GlobalState.Cookies.Where(cookie => cookie.Name == currentCookieName)
                .SingleOrDefault()?.Cookie;
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

                if (string.IsNullOrEmpty(GlobalState.SubscriptionId.SubscriptionId))
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
                GlobalState.CurrentForumName.ForumName = Constants.FavouriteThreadNavigationItemName;

                return;
            }

            #region 检查将要访问的版是否要求持有饼干

            var selectedForumId = args.InvokedItemContainer.DataContext.ToString();
            var currentForumPermissionLevel = GlobalState.ForumAndIdLookup.Values
                .Where(value => value.forumId.ToString() == selectedForumId)
                .Single()
                .permissionLevel;
            if (string.IsNullOrEmpty(GlobalState.CurrentCookie?.CurrentCookie) &&
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
            await RefreshThreads();
        }

        private void OnThreadClicked(object sender, ItemClickEventArgs args)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            var dataContext = ((Grid)args.ClickedItem).DataContext as ThreadDataContext;

            GlobalState.CurrentThreadId = dataContext.ThreadId;
            GlobalState.CurrentThreadIdDisplay.ThreadId = dataContext.ThreadId;
            GlobalState.CurrentThreadAuthorUserHash = dataContext.ThreadAuthorUserHash;

            ButtonPoOnly.IsChecked = false;

            RefreshReplies();
            ContentRepliesGrid.Visibility = Visibility.Visible;

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void OnRefreshThreadButtonClicked(object sender, RoutedEventArgs args)
        {
            if (((NavigationViewItem)ForumListNavigation.SelectedItem).Name ==
                Constants.FavouriteThreadNavigationItemName)
            {
                RefreshSubscriptions();
                return;
            }

            await RefreshThreads();
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

        private void OnForumSelectionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
