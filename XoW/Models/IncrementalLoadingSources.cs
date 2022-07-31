using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Collections;
using XoW.Services;
using XoW.Utils;

namespace XoW.Models
{
    public class TimelineForumThreadSource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var threads = await AnoBbsApiClient.GetTimelineAsync(pageIndex + 1);
            var gridsInTheListView = ComponentsBuilder.BuildGridForThread(threads, GlobalState.CdnUrl);

            return gridsInTheListView;
        }
    }

    public class NormalForumThreadSource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var threads = await AnoBbsApiClient.GetThreadsAsync(GlobalState.CurrentForumId, pageIndex + 1);
            var gridsInTheListView = ComponentsBuilder.BuildGridForThread(threads, GlobalState.CdnUrl);

            return gridsInTheListView;
        }
    }

    public class ThreadReplySource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var replies = await AnoBbsApiClient.GetRepliesAsync(GlobalState.CurrentThreadId, actualPageIndex);
            var grids = actualPageIndex == 1
                ? ComponentsBuilder.BuildGridForReply(
                    replies,
                    GlobalState.CdnUrl)
                : ComponentsBuilder.BuildGridForOnlyReplies(
                    replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(),
                    GlobalState.CdnUrl);

            return grids;
        }
    }

    public class PoOnlyThreadReplySource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var replies = await AnoBbsApiClient.GetPoOnlyRepliesAsync(GlobalState.CurrentThreadId, actualPageIndex);
            var grids = actualPageIndex == 1
                ? ComponentsBuilder.BuildGridForReply(
                    replies,
                    GlobalState.CdnUrl)
                : ComponentsBuilder.BuildGridForOnlyReplies(
                    replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(),
                    GlobalState.CdnUrl);

            return grids;
        }
    }

    public class SubscriptionSource : IIncrementalSource<Grid>
    {
        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var subscriptions =
                await AnoBbsApiClient.GetSubscriptionsAsync(GlobalState.SubscriptionId.SubscriptionId, actualPageIndex);
            var grids = ComponentsBuilder.BuildGrids(subscriptions, GlobalState.CdnUrl, isForSubscription: true);

            return grids;
        }
    }
}
