using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XoW.Models;

namespace XoW.Services
{
    internal static class AnoBbsApiClient
    {
        private const string QueryParamId = "id";
        private const string QueryParamUuid = "uuid";
        private const string QueryParamPageId = "page";
        private const string QueryParamTid = "tid";

        private const string AddSubscriptionSuccessfulMessage = "订阅大成功→_→";
        private const string AddSubscriptionFailedMessage = "该串不存在";

        public static async Task<List<CdnUrl>> GetCdnAsync()
        {
            var responseString = await GetStringResponseAsync(Url.GetCdn);
            var cdnList = JsonConvert.DeserializeObject<List<CdnUrl>>(responseString);

            cdnList = cdnList.OrderBy(entry => entry.Rate).ToList();

            return cdnList;
        }

        public static async Task<List<ForumGroup>> GetForumGroupsAsync()
        {
            var responseString = await GetStringResponseAsync(Url.GetForums);
            var forumGroups = JsonConvert.DeserializeObject<List<ForumGroup>>(responseString);

            return forumGroups;
        }

        public static async Task<List<ForumThread>> GetTimelineAsync(int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetTimeline);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());
            var threads = JsonConvert.DeserializeObject<List<ForumThread>>(responseString);

            return threads;
        }

        public static async Task<List<ForumThread>> GetThreadsAsync(string forumId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetThreads);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamId] = forumId.ToString();
            query[QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());
            var threads = JsonConvert.DeserializeObject<List<ForumThread>>(responseString);

            return threads;
        }

        public static async Task<ThreadReply> GetRepliesAsync(string threadId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetReplies);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamId] = threadId;
            query[QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());
            var reply = JsonConvert.DeserializeObject<ThreadReply>(responseString);

            return reply;
        }

        public static async Task<ThreadReply> GetPoOnlyRepliesAsync(string threadId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetPoOnlyReplies);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamId] = threadId;
            query[QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());
            var reply = JsonConvert.DeserializeObject<ThreadReply>(responseString);

            return reply;
        }

        public static async Task<List<ThreadSubscription>> GetSubscriptionsAsync(string subscriptionId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetSubscription);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamUuid] = subscriptionId;
            query[QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());
            var subscriptions = JsonConvert.DeserializeObject<List<ThreadSubscription>>(responseString);

            return subscriptions;
        }

        public static async Task<string> AddSubscriptionAsync(string subscriptionId, string tid)
        {
            var uriBuilder = new UriBuilder(Url.AddSubscription);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamUuid] = subscriptionId;
            query[QueryParamTid] = tid;
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());

            return JsonConvert.DeserializeObject<string>(responseString);
        }

        private static async Task<string> GetStringResponseAsync(string url)
        {
            var httpClient = HttpClientService.GetHttpClientInstance();

            var uri = new Uri(url);
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var responseJsonObject = JToken.Parse(responseString);
            if (responseJsonObject.SelectToken("success") != null && !responseJsonObject.Value<bool>("success"))
            {
                var errorMessage = responseJsonObject.SelectToken("error").ToString();
                throw new AppException(errorMessage);
            }

            return responseString;
        }
    }
}
