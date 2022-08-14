using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;
using Windows.UI.Xaml.Controls;
using XoW.Services;
using XoW.Utils;
using XoW.Views;

namespace XoW.Models
{
    public class TimelineForumThreadSource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var threads = await AnoBbsApiClient.GetTimelineAsync(pageIndex + 1);
            var gridsInTheListView = await ComponentsBuilder.BuildGridForThread(threads, GlobalState.CdnUrl);

            return gridsInTheListView;
        }
    }

    public class NormalForumThreadSource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var threads = await AnoBbsApiClient.GetThreadsAsync(GlobalState.CurrentForumId, pageIndex + 1);
            var gridsInTheListView = await ComponentsBuilder.BuildGridForThread(threads, GlobalState.CdnUrl);

            return gridsInTheListView;
        }
    }

    public class ThreadReplySource : IIncrementalSource<Grid>
    {
        private int _pageIndex;

        public ThreadReplySource()
        {
            _pageIndex = 1;
        }

        public ThreadReplySource(int pageIndex)
        {
            _pageIndex = pageIndex;
        }

        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var replies = await AnoBbsApiClient.GetRepliesAsync(GlobalState.CurrentThreadId, _pageIndex);
            var threadId = replies.Id;
            var threadAuthorUserHash = replies.UserHash;

            GlobalState.ObservableObject.ThreadId = threadId;
            GlobalState.CurrentThreadAuthorUserHash = threadAuthorUserHash;

            var grids = _pageIndex == 1
                ? await ComponentsBuilder.BuildGridForReply(replies, GlobalState.CdnUrl)
                : await ComponentsBuilder.BuildGridForOnlyReplies(replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(), GlobalState.CdnUrl);

            if (!grids.Any())
            {
                await new NotificationContentDialog(false, ComponentContent.NoMoreReplies).ShowAsync();
                return grids;
            }

            GlobalState.ObservableObject.CurrentPageNumber = _pageIndex;
            _pageIndex++;
            return grids;
        }
    }

    public class PoOnlyThreadReplySource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var replies = await AnoBbsApiClient.GetPoOnlyRepliesAsync(GlobalState.CurrentThreadId, actualPageIndex);
            var grids = actualPageIndex == 1
                ? await ComponentsBuilder.BuildGridForReply(replies, GlobalState.CdnUrl)
                : await ComponentsBuilder.BuildGridForOnlyReplies(replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(), GlobalState.CdnUrl);

            return grids;
        }
    }

    public class SubscriptionSource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var subscriptions = await AnoBbsApiClient.GetSubscriptionsAsync(GlobalState.ObservableObject.SubscriptionId, actualPageIndex);
            var grids = await ComponentsBuilder.BuildGrids(subscriptions, GlobalState.CdnUrl, isForSubscription: true);

            return grids;
        }
    }
}
