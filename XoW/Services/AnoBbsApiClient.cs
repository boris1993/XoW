using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using Windows.Web.Http;
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

        public static async Task<string> DeleteSubscriptionAsync(string subscriptionId, string tid)
        {
            var uriBuilder = new UriBuilder(Url.DeleteSubscription);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParamUuid] = subscriptionId;
            query[QueryParamTid] = tid;
            uriBuilder.Query = query.ToString();

            var responseString = await GetStringResponseAsync(uriBuilder.ToString());

            return JsonConvert.DeserializeObject<string>(responseString);
        }

        public static async Task CreateNewThread(
            string fid,
            string name,
            string email,
            string title,
            string content,
            string water,
            AnoBbsCookie cookie,
            StorageFile image)
        {
            var uri = new Uri(Url.CreateNewThread);

            using var requestBody = new HttpMultipartFormDataContent();
            requestBody.Add(new HttpStringContent(fid), RequestBodyParamName.FId);
            requestBody.Add(new HttpStringContent(content), RequestBodyParamName.Content);
            requestBody.Add(new HttpStringContent(water), RequestBodyParamName.Water);

            if (!string.IsNullOrWhiteSpace(name))
            {
                requestBody.Add(new HttpStringContent(name), RequestBodyParamName.Username);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                requestBody.Add(new HttpStringContent(email), RequestBodyParamName.EMail);
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                requestBody.Add(new HttpStringContent(title), RequestBodyParamName.Title);
            }

            if (image != null)
            {
                var imageStream = await image.OpenReadAsync();
                var fileStreamContent = new HttpStreamContent(imageStream);
                fileStreamContent.Headers.Add(HeaderNames.ContentType, imageStream.ContentType.ToString());
                requestBody.Add(fileStreamContent, RequestBodyParamName.Image, $"/{image.Path.Replace('\\', '/')}");
            }

            var httpClient = HttpClientService.GetHttpClientInstance();

            var defaultCookie = httpClient.DefaultRequestHeaders.Cookie.Single();
            httpClient.DefaultRequestHeaders.Cookie.Clear();
            httpClient.DefaultRequestHeaders.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue(Constants.CookieNameUserHash) { Value = cookie.Cookie });

            var response = await httpClient.PostAsync(uri, requestBody);
            response.EnsureSuccessStatusCode();

            httpClient.DefaultRequestHeaders.Cookie.Clear();
            httpClient.DefaultRequestHeaders.Cookie.Add(defaultCookie);
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
