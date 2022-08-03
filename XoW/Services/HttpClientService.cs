using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace XoW.Services
{
    internal static class HttpClientService
    {
        private const string UserAgent = "HavfunClient-UWP";

        private static readonly object _lock = new object();
        private static HttpClient _httpClient;

        public static HttpClient GetHttpClientInstance()
        {
            if (_httpClient is null)
            {
                lock (_lock)
                {
                    var httpClient = new HttpClient(new HttpBaseProtocolFilter());
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent);

                    _httpClient = httpClient;
                }
            }

            return _httpClient;
        }

        public static void ApplyCookie(string cookieValue)
        {
            var cookie = new HttpCookiePairHeaderValue(Constants.CookieNameUserHash)
            {
                Value = cookieValue
            };

            GetHttpClientInstance().DefaultRequestHeaders.Cookie.Clear();
            GetHttpClientInstance().DefaultRequestHeaders.Cookie.Add(cookie);
        }
    }
}
