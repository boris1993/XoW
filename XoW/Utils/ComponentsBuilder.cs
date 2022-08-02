using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XoW.Models;
using XoW.Services;

namespace XoW.Utils
{
    public static class ComponentsBuilder
    {
        public const string TopLevelStackPanel = "TopLevelStackPanel";
        public const string ThreadHeaderParentGrid = "ThreadHeaderParentGrid";
        public const string ThreadInfoStackPanel = "ThreadInfoStackPanel";
        public const string StackPanelForDeleteButton = "StackPanelForDeleteButton";
        public const string ButtonDeleteSubscriptionName = "ButtonDeleteSubscription";

        public static List<Grid> BuildGridForThread(
            IEnumerable<ForumThread> threads,
            string cdnUrl)
        {
            var gridsInTheListView = BuildGrids(threads, cdnUrl);

            return gridsInTheListView;
        }

        public static List<Grid> BuildGridForReply(
            ThreadReply threadReply,
            string cdnUrl)
        {
            var gridsInTheListView = new List<Grid>();

            #region 渲染第一条串

            var headerForTheFirstGrid = BuildThreadHeader(threadReply, true);
            var contentForTheFirstGrid = BuildThreadContent(threadReply, cdnUrl);
            var firstThreadGrid = BuildThreadParentGrid(threadReply, headerForTheFirstGrid, contentForTheFirstGrid);

            gridsInTheListView.Add(firstThreadGrid);

            #endregion

            #region 渲染回复串

            gridsInTheListView.AddRange(BuildGrids(threadReply.Replies, cdnUrl, true));

            #endregion

            return gridsInTheListView;
        }

        public static List<Grid> BuildGridForOnlyReplies(
            List<ForumThread> replies,
            string cdnUrl) => BuildGrids(replies, cdnUrl, true);

        public static List<Grid> BuildGrids(
            IEnumerable<ForumThread> threads,
            string cdnUrl,
            bool isForReplies = false,
            bool isForSubscription = false)
        {
            var grids = new List<Grid>();

            foreach (var thread in threads)
            {
                var headerStackPanel = BuildThreadHeader(thread, isForReplies, isForSubscription);
                var contentGrid = BuildThreadContent(thread, cdnUrl);

                var grid = BuildThreadParentGrid(thread, headerStackPanel, contentGrid);

                grids.Add(grid);
            }

            return grids;
        }

        /// <summary>
        /// 构建Grid中串头的部分，即串号、饼干、标题、版名、发串时间、SAGE标识
        /// </summary>
        /// <typeparam name="T">类型限定为<see cref="ForumThread"/>及其派生类</typeparam>
        /// <param name="thread">
        /// 一个串或一个回复的对象，即一个<see cref="ForumThread"/>或一个<see cref="ThreadReply"/>对象
        /// </param>
        /// <param name="forumLookup">版名与版ID的映射</param>
        /// <returns>一个串头的<see cref="StackPanel"/></returns>
        public static Grid BuildThreadHeader<T>(
            T thread,
            bool isForReplies = false,
            bool isForSubscription = false)
            where T : ForumThread
        {
            var threadHeaderParentGrid = new Grid
            {
                Name = ThreadHeaderParentGrid,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            threadHeaderParentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            threadHeaderParentGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            var threadHeaderStackPanel = new StackPanel
            {
                Name = ThreadInfoStackPanel,
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            Grid.SetColumn(threadHeaderStackPanel, 0);
            threadHeaderParentGrid.Children.Add(threadHeaderStackPanel);

            var textBlockThreadId = CreateTextBlockWithDefaultMargin($"No.{thread.Id}");
            threadHeaderStackPanel.Children.Add(textBlockThreadId);

            var isSentByAdmin = thread.Admin == "1";
            var textBlockColor = isSentByAdmin ? Colors.Red : Colors.DimGray;
            var textBlockUserHash = CreateTextBlockWithDefaultMargin(thread.UserHash, textBlockColor);
            threadHeaderStackPanel.Children.Add(textBlockUserHash);

            var isPo = thread.UserHash == GlobalState.CurrentThreadAuthorUserHash;
            if (isPo && isForReplies)
            {
                var textBlockPoMark = CreateTextBlockWithDefaultMargin(Constants.Po, Colors.Red);
                threadHeaderStackPanel.Children.Add(textBlockPoMark);
            }

            var textBlockTitle = CreateTextBlockWithDefaultMargin(thread.Title, Colors.Red);
            threadHeaderStackPanel.Children.Add(textBlockTitle);

            var textBlockUserName = CreateTextBlockWithDefaultMargin(thread.Name, Colors.DarkGreen);
            threadHeaderStackPanel.Children.Add(textBlockUserName);

            var forumName = GlobalState.ForumAndIdLookup.Where(f => f.Value.forumId == thread.FId).Select(f => f.Key)
                .FirstOrDefault();
            var textBlockForumName = CreateTextBlockWithDefaultMargin(forumName ?? string.Empty);
            threadHeaderStackPanel.Children.Add(textBlockForumName);

            var textBlockCreatedTime = CreateTextBlockWithDefaultMargin(thread.Now);
            threadHeaderStackPanel.Children.Add(textBlockCreatedTime);

            if (thread.Sage == "1")
            {
                var textBlockSage = CreateTextBlockWithDefaultMargin("(SAGE)", Colors.Red);
                threadHeaderStackPanel.Children.Add(textBlockSage);
            }

            if (isForSubscription)
            {
                var buttonDeleteSubscription = new Button
                {
                    Name = ButtonDeleteSubscriptionName,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Content = new SymbolIcon { Symbol = Symbol.Delete },
                    DataContext = thread.Id,
                };

                var stackPanelForDeleteButton = new StackPanel
                {
                    Name = StackPanelForDeleteButton,
                    Orientation = Orientation.Horizontal,
                    FlowDirection = FlowDirection.RightToLeft,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                Grid.SetColumn(stackPanelForDeleteButton, 1);
                stackPanelForDeleteButton.Children.Add(buttonDeleteSubscription);

                threadHeaderParentGrid.Children.Add(stackPanelForDeleteButton);
            }

            return threadHeaderParentGrid;
        }

        /// <summary>
        /// 构建Grid中串内容的部分，包括图片和正文
        /// </summary>
        /// <typeparam name="T">类型限定为<see cref="ForumThread"/>及其派生类</typeparam>
        /// <param name="thread">
        /// 一个串或一个回复的对象，即一个<see cref="ForumThread"/>或一个<see cref="ThreadReply"/>对象
        /// </param>
        /// <returns>一个串内容的<see cref="Grid"/></returns>
        public static Grid BuildThreadContent<T>(T thread, string cdnUrl) where T : ForumThread
        {
            var contentTextBlocks = HtmlParser.ParseHtmlIntoTextBlocks(thread.Content);

            var contentGridForThisThread = new Grid();
            // 图片列(1/3)
            contentGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { });
            // 内容列(2/3)
            contentGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { });
            contentGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { });

            // 检查有没有图片
            var hasImage = !string.IsNullOrEmpty(thread.Img);

            for (int currentRow = 0; currentRow < contentTextBlocks.Count; currentRow++)
            {
                var textBlockForThisRow = contentTextBlocks[currentRow];

                // 图放在内容左边
                // 所以如果有图，那么内容从第二列开始，横跨两列
                // 图片占1/3位置，内容占2/3，看起来匀称
                var startingColumnOfThisTextBlock = hasImage ? 1 : 0;
                var columnSpanForThisTextBlock = hasImage ? 2 : 3;
                contentGridForThisThread.RowDefinitions.Add(new RowDefinition { });
                Grid.SetRow(textBlockForThisRow, currentRow);
                Grid.SetColumn(textBlockForThisRow, startingColumnOfThisTextBlock);
                Grid.SetColumnSpan(textBlockForThisRow, columnSpanForThisTextBlock);
                contentGridForThisThread.Children.Add(textBlockForThisRow);
            }

            if (hasImage)
            {
                var image = new Image
                {
                    Source = new BitmapImage { UriSource = new Uri($"{cdnUrl}/thumb/{thread.Img}{thread.Ext}") },
                    Stretch = Stretch.None,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(10, 0, 10, 0),
                };

                Grid.SetColumn(image, 0);
                Grid.SetRow(image, 0);
                Grid.SetRowSpan(image, contentGridForThisThread.RowDefinitions.Count);

                contentGridForThisThread.Children.Add(image);
            }

            return contentGridForThisThread;
        }

        public static Grid BuildThreadParentGrid<T>(
            T thread,
            Grid header,
            Grid contentGrid)
            where T : ForumThread
        {
            var parentGridForThisThread = new Grid
            {
                Margin = new Thickness(5),
                DataContext = new ThreadDataContext { ThreadId = thread.Id, ThreadAuthorUserHash = thread.UserHash, },
            };

            var stackPanel = new StackPanel
            {
                Name = TopLevelStackPanel,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            stackPanel.Children.Add(header);
            stackPanel.Children.Add(contentGrid);

            parentGridForThisThread.Children.Add(stackPanel);

            return parentGridForThisThread;
        }

        public static TextBlock CreateTextBlockWithDefaultMargin(string content, Color? color = null) =>
            CreateTextBlock(content, color, new Thickness(0, 0, 10, 10));

        public static TextBlock CreateTextBlock(string content, Color? color = null, Thickness? margin = null)
        {
            var textBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = new SolidColorBrush(color != null ? (Color)color : Colors.DimGray),
                Text = content,
                TextWrapping = TextWrapping.Wrap
            };

            if (margin != null)
            {
                textBlock.Margin = (Thickness)margin;
            }

            return textBlock;
        }
    }
}
