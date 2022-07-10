using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XoW.Models;

namespace XoW.Services
{
    internal static class AnonBbsApiClient
    {
        private const string QueryParamId = "id";
        private const string QueryParamPageId = "page";

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
            query[QueryParamId] = threadId.ToString();
            query[QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());
            var reply = JsonConvert.DeserializeObject<ThreadReply>(responseString);

            return reply;
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
