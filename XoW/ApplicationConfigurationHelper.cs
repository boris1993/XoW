using System.Linq;
using Windows.Storage;
using XoW.Models;

namespace XoW
{
    public static class ApplicationConfigurationHelper
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static void SetCurrentCookie(string cookie)
        {
            localSettings.Values[ApplicationSettingsKey.CurrentCookie] = cookie;
        }

        public static string GetCurrentCookie()
        {
            var currentCookie = localSettings.Values[ApplicationSettingsKey.CurrentCookie]?.ToString() ?? Constants.NoCookieSelected;
            return currentCookie;
        }

        public static void AddCookie(AnonBbsCookie cookie)
        {
            var cookieListComposite = localSettings.Values[ApplicationSettingsKey.AllCookies] as ApplicationDataCompositeValue;
            if (cookieListComposite == null)
            {
                cookieListComposite = new ApplicationDataCompositeValue();
            }

            if (cookieListComposite.ContainsKey(cookie.Name))
            {
                return;
            }

            cookieListComposite[cookie.Name] = cookie.Cookie;

            localSettings.Values[ApplicationSettingsKey.AllCookies] = cookieListComposite;
        }

        public static void LoadAllCookies()
        {
            var cookieListComposite = localSettings.Values[ApplicationSettingsKey.AllCookies] as ApplicationDataCompositeValue;
            if (cookieListComposite == null)
            {
                return;
            }

            var parsedCookies = cookieListComposite
                .Select(cookie => new AnonBbsCookie { Name = cookie.Key, Cookie = cookie.Value.ToString() })
                .ToList();

            parsedCookies.ForEach(cookie => GlobalState.Cookies.Add(cookie));
        }
    }
}
