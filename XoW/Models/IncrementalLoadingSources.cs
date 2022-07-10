using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using XoW.Services;

namespace XoW.Models
{
    public class TimelineForumThreadSource : IIncrementalSource<Grid>
    {
        private readonly List<Grid> _forumThreads;

        public TimelineForumThreadSource()
        {
            _forumThreads = new List<Grid>();
        }

        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var threads = await AnonBbsApiClient.GetTimelineAsync(pageIndex + 1);
            var gridsInTheListView = ComponentsBuilder.BuildGridForThread(threads, MainPage.CdnUrl, MainPage.ForumAndIdLookup);

            return gridsInTheListView;
        }
    }

    public class NormalForumThreadSource : IIncrementalSource<Grid>
    {
        private readonly List<Grid> _forumThreads;

        public NormalForumThreadSource()
        {
            _forumThreads = new List<Grid>();
        }

        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var threads = await AnonBbsApiClient.GetThreadsAsync(MainPage.CurrentForumId, pageIndex + 1);
            var gridsInTheListView = ComponentsBuilder.BuildGridForThread(threads, MainPage.CdnUrl, MainPage.ForumAndIdLookup);

            return gridsInTheListView;
        }
    }

    public class ThreadReplySource : IIncrementalSource<Grid>
    {
        private readonly List<Grid> _replies;

        public ThreadReplySource() => _replies = new List<Grid>();

        public async Task<IEnumerable<Grid>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var actualPageIndex = pageIndex + 1;
            var replies = await AnonBbsApiClient.GetRepliesAsync(MainPage.CurrentThreadId, actualPageIndex);
            var grids = actualPageIndex == 1 ?
                ComponentsBuilder.BuildGridForReply(replies, MainPage.CdnUrl, MainPage.ForumAndIdLookup) :
                ComponentsBuilder.BuildGridForOnlyReplies(replies.Replies.Where(reply => reply.UserHash != "Tips").ToList(), MainPage.CdnUrl, MainPage.ForumAndIdLookup);

            return grids;
        }
    }
}
