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
    public abstract class IncrementalSourceWithPageNumber
    {
        private int _pageIndex;

        public int PageIndex => _pageIndex;

        public IncrementalSourceWithPageNumber()
        {
            _pageIndex = 1;
        }

        public IncrementalSourceWithPageNumber(int pageIndex)
        {
            _pageIndex = pageIndex;
        }

        public void IncreasePageIndex() => _pageIndex++;
    }

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

    public class ThreadReplySource : IncrementalSourceWithPageNumber, IIncrementalSource<Grid>
    {
        public ThreadReplySource() : base()
        {

        }

        public ThreadReplySource(int pageIndex) : base(pageIndex)
        {

        }

        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var replies = await AnoBbsApiClient.GetRepliesAsync(GlobalState.CurrentThreadId, PageIndex);
            var threadId = replies.Id;
            var threadAuthorUserHash = replies.UserHash;

            GlobalState.ObservableObject.ThreadId = threadId;
            GlobalState.CurrentThreadAuthorUserHash = threadAuthorUserHash;

            var grids = PageIndex == 1
                ? await ComponentsBuilder.BuildGridForReply(replies, GlobalState.CdnUrl)
                : await ComponentsBuilder.BuildGridForOnlyReplies(replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(), GlobalState.CdnUrl);

            if (!grids.Any())
            {
                await new NotificationContentDialog(false, ComponentContent.NoMoreReplies).ShowAsync();
                return grids;
            }

            GlobalState.ObservableObject.CurrentPageNumber = PageIndex;
            IncreasePageIndex();
            return grids;
        }
    }

    public class PoOnlyThreadReplySource : IncrementalSourceWithPageNumber, IIncrementalSource<Grid>
    {
        public PoOnlyThreadReplySource() : base()
        {

        }

        public PoOnlyThreadReplySource(int pageIndex) : base(pageIndex)
        {

        }

        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var replies = await AnoBbsApiClient.GetPoOnlyRepliesAsync(GlobalState.CurrentThreadId, actualPageIndex);
            var grids = actualPageIndex == 1
                ? await ComponentsBuilder.BuildGridForReply(replies, GlobalState.CdnUrl)
                : await ComponentsBuilder.BuildGridForOnlyReplies(replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(), GlobalState.CdnUrl);

            if (!grids.Any())
            {
                await new NotificationContentDialog(false, ComponentContent.NoMoreReplies).ShowAsync();
                return grids;
            }

            GlobalState.ObservableObject.CurrentPageNumber = PageIndex;
            IncreasePageIndex();

            return grids;
        }
    }

    public class SubscriptionSource : IncrementalSourceWithPageNumber, IIncrementalSource<Grid>
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
