using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace XoW.Services
{
    internal static class HttpClientService
    {
        private const string UserAgent = "HavfunClient-UWP";

        private static readonly object Lock = new object();
        private static HttpClient _httpClient;
        private static HttpClient _httpClientForThirdParty;

        public static HttpClient GetHttpClientInstance()
        {
            if (_httpClient is null)
            {
                lock (Lock)
                {
                    var httpClient = new HttpClient(new HttpBaseProtocolFilter());
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent);

                    _httpClient = httpClient;
                }
            }

            return _httpClient;
        }
        
        public static HttpClient GetHttpClientForThirdPartyInstance(){
            if (_httpClientForThirdParty is null)
            {
                lock (Lock)
                {
                    _httpClientForThirdParty = new HttpClient();
                }
            }

            return _httpClientForThirdParty;
        }

        public static void ApplyCookie(string cookieValue)
        {
            var cookie = new HttpCookiePairHeaderValue(Constants.CookieNameUserHash)
            {
                Value = cookieValue,
            };

            GetHttpClientInstance().DefaultRequestHeaders.Cookie.Clear();
            GetHttpClientInstance().DefaultRequestHeaders.Cookie.Add(cookie);
        }
    }
}
