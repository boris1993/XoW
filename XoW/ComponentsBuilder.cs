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
        public const string headerGridName = "threadHeaderGrid";
        public const string contentGridName = "threadContentGrid";

        public static List<Grid> BuildGridForThread(IEnumerable<ForumThread> threads, string cdnUrl, Dictionary<string, int> forumLookup)
        {
            var gridsInTheListView = new List<Grid>();

            // Need 2 Grids here
            // One for the header
            // another one for the content
            foreach (var thread in threads)
            {
                var parentGridForThisThread = BuildThreadParentGrid(thread);

                #region 第一行，包括饼干，标题，发串日期
                var headerGridForThisThread = BuildThreadHeaderGrid(thread, forumLookup);
                parentGridForThisThread.Children.Add(headerGridForThisThread);
                #endregion

                #region 串的内容
                var contentGridForThisThread = BuildThreadContentGrid(thread, cdnUrl);
                parentGridForThisThread.Children.Add(contentGridForThisThread);
                #endregion

                gridsInTheListView.Add(parentGridForThisThread);
            }

            return gridsInTheListView;
        }

        public static List<Grid> BuildGridForReply(ThreadReply threadReply, string cdnUrl, Dictionary<string, int> forumLookup)
        {
            var gridsInTheListView = new List<Grid>();

            var firstThreadGrid = BuildThreadParentGrid(threadReply);
            var headerGridForTheFirstGrid = BuildThreadHeaderGrid(threadReply, forumLookup);
            var contentGridForTheFirstGrid = BuildThreadContentGrid(threadReply, cdnUrl);
            firstThreadGrid.Children.Add(headerGridForTheFirstGrid);
            firstThreadGrid.Children.Add(contentGridForTheFirstGrid);

            gridsInTheListView.Add(firstThreadGrid);

            foreach (var reply in threadReply.Replies)
            {
                var headerGrid = BuildThreadHeaderGrid(reply, forumLookup);
                var contentGrid = BuildThreadContentGrid(reply, cdnUrl);

                var replyGrid = BuildThreadParentGrid(reply);
                replyGrid.Children.Add(headerGrid);
                replyGrid.Children.Add(contentGrid);

                gridsInTheListView.Add(replyGrid);
            }

            return gridsInTheListView;
        }

        /// <summary>
        /// 构建Grid中串头的部分
        /// </summary>
        /// <typeparam name="T">类型限定为<see cref="ForumThread"/>及其派生类</typeparam>
        /// <param name="thread">
        /// 一个串或一个回复的对象，即一个<see cref="ForumThread"/>或一个<see cref="ThreadReply"/>对象
        /// </param>
        /// <param name="forumLookup">版名与版ID的映射</param>
        /// <returns>一个串头的<see cref="Grid"/></returns>
        public static Grid BuildThreadHeaderGrid<T>(T thread, Dictionary<string, int> forumLookup) where T : ForumThread
        {
            var headerGridForThisThread = new Grid { Name = headerGridName };

            // 串号
            headerGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            // 饼干列
            headerGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            // 标题列
            headerGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            // 所属板块
            headerGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            // 发串日期列
            headerGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            // Sage
            headerGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            // 串头唯一的一行
            headerGridForThisThread.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var textBlockThreadId = new TextBlock
            {
                Name = "textBlockThreadId",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 10, 10),
                DataContext = thread.Id,
                Text = $"No.{thread.Id}",
            };
            Grid.SetRow(textBlockThreadId, 0);
            Grid.SetColumn(textBlockThreadId, 0);

            var isSentByAdmin = thread.Admin == 1;
            var textBlockUserHash = new TextBlock
            {
                Name = "textBlockUserHash",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = isSentByAdmin ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 10, 0),
                Text = thread.UserHash,
            };
            Grid.SetRow(textBlockUserHash, 0);
            Grid.SetColumn(textBlockUserHash, 1);

            var textBlockTitle = new TextBlock
            {
                Name = "textBlockTitle",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Red),
                Margin = new Thickness(0, 0, 10, 10),
                Text = thread.Title,
            };
            Grid.SetRow(textBlockTitle, 0);
            Grid.SetColumn(textBlockTitle, 2);

            var forumName = forumLookup.Where(f => f.Value == thread.FId).Select(f => f.Key).FirstOrDefault();
            var textBlockForumName = new TextBlock
            {
                Name = "textBlockForumName",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 10, 10),
                Text = forumName,
            };
            Grid.SetRow(textBlockForumName, 0);
            Grid.SetColumn(textBlockForumName, 3);

            var textBlockCreatedTime = new TextBlock
            {
                Name = "textBlockCreatedTime",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 10, 10),
                Text = thread.Now,
            };
            Grid.SetRow(textBlockCreatedTime, 0);
            Grid.SetColumn(textBlockCreatedTime, 4);

            if (thread.Sage == 1)
            {
                var textBlockSage = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Foreground = new SolidColorBrush(Colors.Red),
                    Margin = new Thickness(0, 0, 10, 10),
                    Text = "(SAGE)",
                };
                Grid.SetRow(textBlockSage, 0);
                Grid.SetColumn(textBlockSage, 5);
                headerGridForThisThread.Children.Add(textBlockSage);
            }

            Grid.SetRow(headerGridForThisThread, 0);
            Grid.SetColumn(headerGridForThisThread, 0);
            headerGridForThisThread.Children.Add(textBlockUserHash);
            headerGridForThisThread.Children.Add(textBlockTitle);
            headerGridForThisThread.Children.Add(textBlockForumName);
            headerGridForThisThread.Children.Add(textBlockCreatedTime);
            headerGridForThisThread.Children.Add(textBlockThreadId);

            return headerGridForThisThread;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thread"></param>
        /// <returns></returns>
        public static Grid BuildThreadContentGrid<T>(T thread, string cdnUrl) where T : ForumThread
        {
            var contentTextBlocks = HtmlParser.ParseHtmlIntoTextBlocks(thread.Content);

            var contentGridForThisThread = new Grid { Name = contentGridName };
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
                    Stretch = Stretch.Uniform
                };

                var gridForImage = new Grid
                {
                    Margin = new Thickness(5)
                };
                gridForImage.Children.Add(image);
                Grid.SetColumn(gridForImage, 0);
                Grid.SetRowSpan(gridForImage, contentGridForThisThread.RowDefinitions.Count);

                contentGridForThisThread.Children.Add(gridForImage);
            }

            Grid.SetRow(contentGridForThisThread, 1);
            Grid.SetColumn(contentGridForThisThread, 0);

            return contentGridForThisThread;
        }

        public static Grid BuildThreadParentGrid<T>(T thread) where T : ForumThread
        {
            var parentGridForThisThread = new Grid
            {
                Margin = new Thickness(5),
                DataContext = thread.Id,
            };

            parentGridForThisThread.ColumnDefinitions.Add(new ColumnDefinition { });

            // 第一行，包括饼干，标题，发串日期
            parentGridForThisThread.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            // 串的内容
            parentGridForThisThread.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            return parentGridForThisThread;
        }

        public static TextBlock CreateTextBlock(string content) => CreateTextBlock(content, Colors.DimGray);

        public static TextBlock CreateTextBlock(string content, Color color) =>
            new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = new SolidColorBrush(color),
                Text = content,
                TextWrapping = TextWrapping.Wrap
            };
    }
}
