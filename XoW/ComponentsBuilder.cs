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

namespace XoW
{
    public static class ComponentsBuilder
    {
        public static List<Grid> BuildGridForThread(
            IEnumerable<ForumThread> threads,
            string cdnUrl,
            Dictionary<string, (string, string)> forumLookup)
        {
            var gridsInTheListView = BuildGrids(threads, cdnUrl, forumLookup);

            return gridsInTheListView;
        }

        public static List<Grid> BuildGridForReply(
            ThreadReply threadReply,
            string cdnUrl,
            Dictionary<string, (string, string)> forumLookup)
        {
            var gridsInTheListView = new List<Grid>();

            #region 渲染第一条串
            var headerForTheFirstGrid = BuildThreadHeader(threadReply, forumLookup);
            var contentForTheFirstGrid = BuildThreadContent(threadReply, cdnUrl);
            var firstThreadGrid = BuildThreadParentGrid(threadReply, headerForTheFirstGrid, contentForTheFirstGrid);

            gridsInTheListView.Add(firstThreadGrid);
            #endregion

            #region 渲染回复串
            gridsInTheListView.AddRange(BuildGrids(threadReply.Replies, cdnUrl, forumLookup));
            #endregion

            return gridsInTheListView;
        }

        public static List<Grid> BuildGridForOnlyReplies(
            List<ForumThread> replies,
            string cdnUrl,
            Dictionary<string, (string, string)> forumLookup) => BuildGrids(replies, cdnUrl, forumLookup);

        public static List<Grid> BuildGrids(
            IEnumerable<ForumThread> threads,
            string cdnUrl,
            Dictionary<string, (string, string)> forumLookup)
        {
            var grids = new List<Grid>();

            foreach (var thread in threads)
            {
                var threadId = thread.Id;
                var headerStackPanel = BuildThreadHeader(thread, forumLookup);
                var contentGrid = BuildThreadContent(thread, cdnUrl);

                var grid = BuildThreadParentGrid(thread, headerStackPanel, contentGrid);
                grid.DataContext = threadId;

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
        public static StackPanel BuildThreadHeader<T>(
            T thread,
            Dictionary<string, (string forumId, string permissionLevel)> forumLookup)
            where T : ForumThread
        {
            var threadHeaderStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };

            var textBlockThreadId = CreateTextBlockWithDefaultMargin($"No.{thread.Id}");
            threadHeaderStackPanel.Children.Add(textBlockThreadId);

            var isSentByAdmin = thread.Admin == "1";
            var textBlockColor = isSentByAdmin ? Colors.Red : Colors.DimGray;
            var textBlockUserHash = CreateTextBlockWithDefaultMargin(thread.UserHash, textBlockColor);
            threadHeaderStackPanel.Children.Add(textBlockUserHash);

            var textBlockTitle = CreateTextBlockWithDefaultMargin(thread.Title, Colors.Red);
            threadHeaderStackPanel.Children.Add(textBlockTitle);

            var textBlockUserName = CreateTextBlockWithDefaultMargin(thread.Name, Colors.DarkGreen);
            threadHeaderStackPanel.Children.Add(textBlockUserName);

            var forumName = forumLookup.Where(f => f.Value.forumId == thread.FId).Select(f => f.Key).FirstOrDefault();
            var textBlockForumName = CreateTextBlockWithDefaultMargin(forumName ?? string.Empty);
            threadHeaderStackPanel.Children.Add(textBlockForumName);

            var textBlockCreatedTime = CreateTextBlockWithDefaultMargin(thread.Now);
            threadHeaderStackPanel.Children.Add(textBlockCreatedTime);

            if (thread.Sage == "1")
            {
                var textBlockSage = CreateTextBlockWithDefaultMargin("(SAGE)", Colors.Red);
                threadHeaderStackPanel.Children.Add(textBlockSage);
            }

            return threadHeaderStackPanel;
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
            StackPanel headerStackPanel,
            Grid contentGrid)
            where T : ForumThread
        {
            var parentGridForThisThread = new Grid
            {
                Margin = new Thickness(5),
                DataContext = thread.Id,
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };
            stackPanel.Children.Add(headerStackPanel);
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
