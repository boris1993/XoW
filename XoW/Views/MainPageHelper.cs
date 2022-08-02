using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XoW.Models;
using XoW.Services;
using XoW.Utils;

namespace XoW.Views
{
    partial class MainPage : Page
    {
        private static async Task<string> GetCdnUrl() => (await AnoBbsApiClient.GetCdnAsync()).First().Url;

        private async Task RefreshForumsAsync()
        {
            GlobalState.ForumAndIdLookup = new Dictionary<string, (string, string)>();

            var forumGroups = await AnoBbsApiClient.GetForumGroupsAsync();

            // 版面组和版面按照Sort排序，保证以正确的顺序展示
            forumGroups = forumGroups
                .OrderBy(fg => fg.Sort)
                .ToList();

            forumGroups.ForEach(fg =>
            {
                fg.Forums = fg.Forums.OrderBy(f => f.Sort).ToList();
            });

            forumGroups
                .SelectMany(fg => fg.Forums)
                .ToList()
                .ForEach(f => GlobalState.ForumAndIdLookup.Add(f.Name, (f.Id, f.permissionLevel)));

            _navigationItems.Clear();

            var favouriteThreadsNavigationItem = new NavigationViewItem
            {
                Content = Constants.FavouriteThreadNavigationItemName,
                Name = Constants.FavouriteThreadNavigationItemName,
                Icon = new SymbolIcon(Symbol.OutlineStar),
            };
            _navigationItems.Add(favouriteThreadsNavigationItem);

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
            ForumListNavigation.SelectedItem = _navigationItems
                .First(item => item is NavigationViewItem && !_nonForumNavigationItems.Contains(item.Name));
        }

        private void RefreshThreads()
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            if (GlobalState.CurrentForumId == Constants.TimelineForumId)
            {
                ThreadsListView.ItemsSource = new IncrementalLoadingCollection<TimelineForumThreadSource, Grid>();
            }
            else
            {
                ThreadsListView.ItemsSource = new IncrementalLoadingCollection<NormalForumThreadSource, Grid>();
            }

            GlobalState.ObservableObject.ForumName = GlobalState.ForumAndIdLookup
                .Single(lookup => lookup.Value.forumId == GlobalState.CurrentForumId)
                .Key;

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private void RefreshReplies()
        {
            RepliesListView.ItemsSource = new IncrementalLoadingCollection<ThreadReplySource, Grid>();
        }

        private void RefreshPoOnlyReplies()
        {
            RepliesListView.ItemsSource = new IncrementalLoadingCollection<PoOnlyThreadReplySource, Grid>();
        }

        private void RefreshSubscriptions()
        {
            MainPageProgressBar.Visibility = Visibility.Visible;

            var itemsSource = new IncrementalLoadingCollection<SubscriptionSource, Grid>();
            itemsSource.OnEndLoading = () =>
            {
                foreach (var item in itemsSource)
                {
                    var contentParentStackPanel = item.Children
                        .Single(element =>
                            ((StackPanel)element).Name == ComponentsBuilder.TopLevelStackPanel) as StackPanel;

                    var headerGrid = contentParentStackPanel.Children
                        .Single(element => ((Grid)element).Name == ComponentsBuilder.ThreadHeaderParentGrid) as Grid;

                    var stackPanelForDeleteButton = headerGrid.Children
                        .Single(element =>
                            ((StackPanel)element).Name == ComponentsBuilder.StackPanelForDeleteButton) as StackPanel;

                    var buttonForDeleteSubscription = stackPanelForDeleteButton
                        .Children
                        .Where(element => element is Button)
                        .Single(button =>
                            ((Button)button).Name == ComponentsBuilder.ButtonDeleteSubscriptionName) as Button;

                    // 确保这个EventHandler只被注册一次
                    buttonForDeleteSubscription.Click -= OnDeleteSubscriptionButtonClicked;
                    buttonForDeleteSubscription.Click += OnDeleteSubscriptionButtonClicked;
                }
            };

            ThreadsListView.ItemsSource = itemsSource;

            MainPageProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowSettingsGrid()
        {
            ContentGrid.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Visible;
        }

        private void ShowContentGrid()
        {
            ContentGrid.Visibility = Visibility.Visible;
            SettingsPage.Visibility = Visibility.Collapsed;
        }

        private void ShowNewThreadPanel()
        {
            //ContentThreadGrid.Visibility = Visibility.Collapsed;

            NewThreadPanelGrid.Visibility = Visibility.Visible;
        }

        private void HideNewThreadPanel()
        {
            ResetNewThreadPanel();
            NewThreadPanelGrid.Visibility = Visibility.Collapsed;
        }

        private void ResetNewThreadPanel()
        {
            TextBoxNewThreadUserName.Text = "";
            TextBoxNewThreadEmail.Text = "";
            TextBoxNewThreadTitle.Text = "";
            TextBoxNewThreadContent.Text = "";
            NewThreadCookieSelectionComboBox.SelectedItem = GlobalState.Cookies.Single(cookie => cookie.Name == GlobalState.ObservableObject.CurrentCookie);
            ForumSelectionComboBox.SelectedIndex = 0;
            ButtonNewThreadAttachPicture.DataContext = null;
            CheckBoxNewThreadWaterMark.IsChecked = true;
            ImagePreviewStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
