using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XoW.Models;

namespace XoW
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ObservableCollection<NavigationViewItemBase> _navigationItems = new ObservableCollection<NavigationViewItemBase>();
        private readonly ObservableCollection<ForumThread> _threads = new ObservableCollection<ForumThread>();
        private readonly Dictionary<string, int> _forumAndIdLookup = new Dictionary<string, int>();

        private string _cdnUrl;
        private string _currentForumId = "-1";
        private string _currentThreadId = "";
        private ObservableCurrentThreadPage _currentThreadPage = new ObservableCurrentThreadPage { CurrentPage = 1 };

        public MainPage()
        {
            InitializeComponent();

            ThreadPageNumberTextBox.NumberFormatter = new DecimalFormatter
            {
                FractionDigits = 0,
                NumberRounder = new IncrementNumberRounder
                {
                    RoundingAlgorithm = RoundingAlgorithm.RoundDown
                },
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            _cdnUrl = await GetCdnUrl();
            // 刷新板块列表，完成后默认选定时间线版
            await RefreshForumsAsync();
            // 载入时间线第一页
            await RefreshThreads();

            _currentForumId = Constants.TimelineForumId;

            MainPageProgressBar.Visibility = Visibility.Collapsed;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[Constants.SettingsKeyCdn] = _cdnUrl;
        }

        private async void NavigationItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Go to the settings page
                return;
            }

            _currentForumId = args.InvokedItemContainer.DataContext.ToString();
            await RefreshThreads();
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

            _currentThreadId = threadId;

            await RefreshReplies();
            Replies.Visibility = Visibility.Visible;
            ReplyTopBar.Visibility = Visibility.Visible;

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void OnRefreshThreadButtonClicked(object sender, RoutedEventArgs args) => await RefreshThreads();

        private async void OnRefreshRepliesButtonClicked(object sender, RoutedEventArgs args) => await RefreshReplies();

        private async void OnThreadPrevPageButtonClicked(object sender, RoutedEventArgs args)
        {
            ButtonThreadPrevPage.IsEnabled = false;
            ButtonThreadNextPage.IsEnabled = false;

            _currentThreadPage.CurrentPage--;

            if (_currentThreadPage.CurrentPage > 1)
            {
                ButtonThreadPrevPage.IsEnabled = true;
            }

            ButtonThreadNextPage.IsEnabled = true;
        }

        private async void OnThreadNextPageButtonClicked(object sender, RoutedEventArgs args)
        {
            ButtonThreadPrevPage.IsEnabled = false;
            ButtonThreadNextPage.IsEnabled = false;

            _currentThreadPage.CurrentPage++;

            if (_currentThreadPage.CurrentPage > 1)
            {
                ButtonThreadPrevPage.IsEnabled = true;
            }

            ButtonThreadNextPage.IsEnabled = true;
        }
    }
}
