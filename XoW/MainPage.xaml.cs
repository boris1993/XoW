using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XoW.Models;
using XoW.Services;

namespace XoW
{
    public sealed partial class MainPage : Page
    {
        private readonly ObservableCollection<NavigationViewItemBase> _navigationItems = new ObservableCollection<NavigationViewItemBase>();
        private readonly ObservableCollection<ForumThread> _threads = new ObservableCollection<ForumThread>();
        private readonly ObservableCollection<ForumThread> _replies = new ObservableCollection<ForumThread>();
        private readonly Dictionary<string, int> _forumAndIdLookup = new Dictionary<string, int>();

        private string _cdnUrl;
        private string currentForumId = "-1";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            _cdnUrl = await GetCdnUrl();
            // 刷新板块列表，完成后默认选定时间线版
            await RefreshForumsAsync();
            // 载入时间线第一页
            await RefreshThreads();

            currentForumId = Constants.TimelineForumId;

            MainPageProgressBar.Visibility = Visibility.Collapsed;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[Constants.SettingsKeyCdn] = _cdnUrl;
        }

        private async Task<string> GetCdnUrl() => (await AnonBbsApiClient.GetCdnAsync()).First().Url;

        private async Task RefreshForumsAsync()
        {
            var forumGroups = await AnonBbsApiClient.GetForumGroupsAsync();

            forumGroups
                .SelectMany(fg => fg.Forums)
                .ToList()
                .ForEach(f => _forumAndIdLookup.Add(f.Name, f.Id));

            // 版面组和版面按照Sort排序，保证以正确的顺序展示
            forumGroups.OrderBy(fg =>
            {
                fg.Forums = fg.Forums.OrderBy(f => f.Sort).ToList();
                return fg;
            });

            _navigationItems.Clear();

            forumGroups.ForEach(fg =>
                {
                    // 版面组名作为导航栏Header
                    var navigationHeader = new NavigationViewItemHeader
                    {
                        Content = fg.Name,
                        Name = fg.Name,
                        // 在导航栏折叠时，隐藏导航栏Header
                        Visibility = ForumListNavigation.IsPaneOpen ? Visibility.Visible : Visibility.Collapsed,
                    };
                    _navigationItems.Add(navigationHeader);

                    // 遍历版面组下的版面，依次插入导航栏
                    fg.Forums.ToList().ForEach(f =>
                    {
                        var navigationItem = new NavigationViewItem
                        {
                            Content = f.Name,
                            Name = f.Name,
                            // 取版面名第一个字作为图标，在导航栏折叠时展示
                            Icon = new FontIcon
                            {
                                // 是个Windows肯定会带微软雅黑的吧
                                FontFamily = new FontFamily("Microsoft YaHei"),
                                Glyph = f.Name.First().ToString()
                            },
                            DataContext = f.Id.ToString(),
                        };
                        _navigationItems.Add(navigationItem);
                    });

                    _navigationItems.Add(new NavigationViewItemSeparator());
                });

            // 版面导航栏加载完成后，默认选择第一项，即默认展示时间线
            ForumListNavigation.SelectedItem = _navigationItems.Where(item => item is NavigationViewItem).First();
        }

        private async Task RefreshThreads()
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            _threads.Clear();

            if (currentForumId == Constants.TimelineForumId)
            {
                ButtonCreateThread.IsEnabled = false;
                await LoadTimeline();
                GenerateThreadsInListView();
            }
            else
            {
                ButtonCreateThread.IsEnabled = true;
                (await AnonBbsApiClient.GetThreadsAsync(currentForumId)).ForEach(ft => _threads.Add(ft));
                GenerateThreadsInListView();
            }

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async Task LoadTimeline()
        {
            _threads.Clear();

            var threads = await AnonBbsApiClient.GetTimelineAsync();
            threads.ForEach(t => _threads.Add(t));

            // 展示时间线的串
            GenerateThreadsInListView();
        }

        private void GenerateThreadsInListView()
        {
            var gridsInTheListView = ComponentsBuilder.BuildGridForThread(_threads, _cdnUrl, _forumAndIdLookup);
            ThreadsListView.ItemsSource = gridsInTheListView;
        }

        private async void NavigationItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Go to the settings page

            }

            currentForumId = args.InvokedItemContainer.DataContext.ToString();
            await RefreshThreads();
        }

        private async void OnThreadClicked(object sender, ItemClickEventArgs args)
        {
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

            var replies = await AnonBbsApiClient.GetReplies(threadId, 1);


        }

        private async void OnRefreshThreadButtonClicked(object sender, RoutedEventArgs e) => await RefreshThreads();
    }
}
