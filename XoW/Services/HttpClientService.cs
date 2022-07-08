using Windows.Web.Http;

namespace XoW.Services
{
    internal static class HttpClientService
    {
        private static object _lock = new object();
        private static HttpClient _httpClient;

        public static HttpClient GetHttpClientInstance()
        {
            if (_httpClient is null)
            {
                lock (_lock)
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.UserAgent);

                    _httpClient = httpClient;
                }
            }

            return _httpClient;
        }
    }
}
