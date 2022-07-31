using System.Linq;
using Windows.Storage;
using XoW.Models;

namespace XoW.Utils
{
    public static class ApplicationConfigurationHelper
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static void SetCurrentCookie(string cookie)
        {
            localSettings.Values[ApplicationSettingsKey.CurrentCookie] = cookie;
            GlobalState.ObservableObject.CurrentCookie = cookie;
        }

        public static string GetCurrentCookie()
        {
            var currentCookie = localSettings.Values[ApplicationSettingsKey.CurrentCookie]?.ToString();
            return currentCookie;
        }

        public static void RemoveCurrentCookie()
        {
            localSettings.Values.Remove(ApplicationSettingsKey.CurrentCookie);
        }

        public static void AddCookie(AnoBbsCookie cookie)
        {
            var cookieListComposite =
                localSettings.Values[ApplicationSettingsKey.AllCookies] as ApplicationDataCompositeValue;
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

        public static void DeleteCookie(string cookieName)
        {
            var cookieListComposite =
                localSettings.Values[ApplicationSettingsKey.AllCookies] as ApplicationDataCompositeValue;
            if (cookieListComposite == null)
            {
                return;
            }

            cookieListComposite.Remove(cookieName);

            localSettings.Values[ApplicationSettingsKey.AllCookies] = cookieListComposite;
        }

        public static void LoadAllCookies()
        {
            var cookieListComposite =
                localSettings.Values[ApplicationSettingsKey.AllCookies] as ApplicationDataCompositeValue;
            if (cookieListComposite == null)
            {
                return;
            }

            var parsedCookies = cookieListComposite
                .Select(cookie => new AnoBbsCookie {Name = cookie.Key, Cookie = cookie.Value.ToString()})
                .ToList();

            parsedCookies.ForEach(cookie => GlobalState.Cookies.Add(cookie));
        }

        public static void SetDarkThemeEnabled(bool isDarkThemeEnabled)
        {
            localSettings.Values[ApplicationSettingsKey.DarkThemeSelected] = isDarkThemeEnabled;
        }

        public static bool IsDarkThemeEnabled()
        {
            return localSettings.Values.ContainsKey(ApplicationSettingsKey.DarkThemeSelected) &&
                   (bool)localSettings.Values[ApplicationSettingsKey.DarkThemeSelected];
        }

        public static void SetSubscriptionId(string subscriptionId)
        {
            localSettings.Values[ApplicationSettingsKey.SubscriptionId] = subscriptionId;
        }

        public static string GetSubscriptionId()
        {
            return localSettings.Values.ContainsKey(ApplicationSettingsKey.SubscriptionId)
                ? localSettings.Values[ApplicationSettingsKey.SubscriptionId].ToString()
                : null;
        }
    }
}
