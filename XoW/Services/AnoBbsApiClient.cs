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
using Windows.Web.Http.Headers;
using XoW.Models;

namespace XoW.Services
{
    public static class AnoBbsApiClient
    {
        /// <summary>
        /// 值班室版面ID
        /// </summary>
        private const string ForumIdDutyRoom = "18";

        public static async Task<List<CdnUrl>> GetCdnAsync()
        {
            var cdnList = await GetResponseWithType<List<CdnUrl>>(Url.GetCdn);
            cdnList = cdnList.OrderBy(entry => entry.Rate).ToList();

            return cdnList;
        }

        public static async Task<List<ForumGroup>> GetForumGroupsAsync()
        {
            var forumGroups = await GetResponseWithType<List<ForumGroup>>(Url.GetForums);
            return forumGroups;
        }

        public static async Task<List<ForumThread>> GetTimelineAsync(int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetTimeline);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var threads = await GetResponseWithType<List<ForumThread>>(uriBuilder.ToString());

            return threads;
        }

        public static async Task<List<ForumThread>> GetThreadsAsync(string forumId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetThreads);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamId] = forumId;
            query[QueryParams.QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var threads = await GetResponseWithType<List<ForumThread>>(uriBuilder.ToString());

            return threads;
        }

        public static async Task<ThreadReply> GetRepliesAsync(string threadId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetReplies);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamId] = threadId;
            query[QueryParams.QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var reply = await GetResponseWithType<ThreadReply>(uriBuilder.ToString());

            return reply;
        }

        public static async Task<ThreadReply> GetPoOnlyRepliesAsync(string threadId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetPoOnlyReplies);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamId] = threadId;
            query[QueryParams.QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var reply = await GetResponseWithType<ThreadReply>(uriBuilder.ToString());

            return reply;
        }

        public static async Task<List<ThreadSubscription>> GetSubscriptionsAsync(string subscriptionId, int pageId = 1)
        {
            var uriBuilder = new UriBuilder(Url.GetSubscription);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamUuid] = subscriptionId;
            query[QueryParams.QueryParamPageId] = pageId.ToString();
            uriBuilder.Query = query.ToString();

            var subscriptions = await GetResponseWithType<List<ThreadSubscription>>(uriBuilder.ToString());

            return subscriptions;
        }

        public static async Task<string> AddSubscriptionAsync(string subscriptionId, string tid)
        {
            var uriBuilder = new UriBuilder(Url.AddSubscription);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamUuid] = subscriptionId;
            query[QueryParams.QueryParamTid] = tid;
            uriBuilder.Query = query.ToString();

            return await GetResponseWithType<string>(uriBuilder.ToString());
        }

        public static async Task<string> DeleteSubscriptionAsync(string subscriptionId, string tid)
        {
            var uriBuilder = new UriBuilder(Url.DeleteSubscription);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamUuid] = subscriptionId;
            query[QueryParams.QueryParamTid] = tid;
            uriBuilder.Query = query.ToString();

            return await GetResponseWithType<string>(uriBuilder.ToString());
        }

        /// <summary>
        /// 搜索暂未开放，先留一个stub在这里，开放后再适配
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<string> SearchThread(string keyword, string page = "1")
        {
            var uriBuilder = new UriBuilder(Url.Search);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[QueryParams.QueryParamKeyword] = keyword;
            query[QueryParams.QueryParamPageId] = page;
            uriBuilder.Query = query.ToString();

            return await GetResponseWithType<string>(uriBuilder.ToString());
        }

        public static async Task PostNewReport(string reportThreadContent)
        {
            await CreateNewThread(ForumIdDutyRoom,
                null,
                null,
                null,
                reportThreadContent,
                "0",
                GlobalState.Cookies.Single(cookie => cookie.Name == GlobalState.ObservableObject.CurrentCookie),
                null);
        }

        public static async Task CreateNewThread(string fid, string name, string email, string title, string content, string water, AnoBbsCookie cookie, StorageFile image)
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
                fileStreamContent.Headers.Add(HeaderNames.ContentType, imageStream.ContentType);
                requestBody.Add(fileStreamContent, RequestBodyParamName.Image, $"/{image.Path.Replace('\\', '/')}");
            }

            var httpClient = HttpClientService.GetHttpClientInstance();

            var defaultCookie = httpClient.DefaultRequestHeaders.Cookie.Single();
            httpClient.DefaultRequestHeaders.Cookie.Clear();
            httpClient.DefaultRequestHeaders.Cookie.Add(new HttpCookiePairHeaderValue(Constants.CookieNameUserHash)
            {
                Value = cookie.Cookie,
            });

            var response = await httpClient.PostAsync(uri, requestBody);
            response.EnsureSuccessStatusCode();

            httpClient.DefaultRequestHeaders.Cookie.Clear();
            httpClient.DefaultRequestHeaders.Cookie.Add(defaultCookie);
        }

        public static async Task CreateNewReply(string resto, string name, string email, string title, string content, string water, AnoBbsCookie cookie, StorageFile image)
        {
            var uri = new Uri(Url.CreateNewReply);

            using var requestBody = new HttpMultipartFormDataContent();
            requestBody.Add(new HttpStringContent(resto), RequestBodyParamName.Resto);
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
                fileStreamContent.Headers.Add(HeaderNames.ContentType, imageStream.ContentType);
                requestBody.Add(fileStreamContent, RequestBodyParamName.Image, $"/{image.Path.Replace('\\', '/')}");
            }

            var httpClient = HttpClientService.GetHttpClientInstance();

            var defaultCookie = httpClient.DefaultRequestHeaders.Cookie.Single();
            httpClient.DefaultRequestHeaders.Cookie.Clear();
            httpClient.DefaultRequestHeaders.Cookie.Add(new HttpCookiePairHeaderValue(Constants.CookieNameUserHash)
            {
                Value = cookie.Cookie,
            });

            var response = await httpClient.PostAsync(uri, requestBody);
            response.EnsureSuccessStatusCode();

            httpClient.DefaultRequestHeaders.Cookie.Clear();
            httpClient.DefaultRequestHeaders.Cookie.Add(defaultCookie);
        }

        private static async Task<T> GetResponseWithType<T>(string url)
        {
            var httpClient = HttpClientService.GetHttpClientInstance();

            var uri = new Uri(url);
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            HandlePotentialErrorMessage(responseString);

            try
            {
                var returnObject = JsonConvert.DeserializeObject<T>(responseString);
                return returnObject;
            }
            catch (JsonSerializationException)
            {
                var errorMessage = JsonConvert.DeserializeObject<string>(responseString);
                throw new AppException(errorMessage);
            }
        }

        // ReSharper disable InvertIf
        private static void HandlePotentialErrorMessage(string responseString)
        {
            var responseJsonObject = JToken.Parse(responseString);
            if (responseJsonObject.SelectToken("success") != null && !responseJsonObject.Value<bool>("success"))
            {
                var errorMessage = responseJsonObject.SelectToken("error")?.ToString() ?? responseString;
                throw new AppException(errorMessage);
            }

            if (responseJsonObject.SelectToken("msg") != null)
            {
                var errorMessage = responseJsonObject.SelectToken("msg")?.ToString() ?? responseString;
                throw new AppException(errorMessage);
            }
        }
    }

    public static class QueryParams
    {
        public const string QueryParamId = "id";
        public const string QueryParamUuid = "uuid";
        public const string QueryParamPageId = "page";
        public const string QueryParamTid = "tid";
        public const string QueryParamKeyword = "q";
    }

    public static class RequestBodyParamName
    {
        public const string FId = "fid";
        public const string Resto = "resto";
        public const string Username = "name";
        public const string EMail = "email";
        public const string Title = "title";
        public const string Content = "content";
        public const string Water = "water";
        public const string Image = "image";
    }
}
