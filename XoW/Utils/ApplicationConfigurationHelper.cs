using System.Linq;
using Windows.Storage;
using XoW.Models;

namespace XoW.Utils
{
    public static class ApplicationConfigurationHelper
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public static void SetCurrentCookie(string cookie)
        {
            LocalSettings.Values[ApplicationSettingsKey.CurrentCookie] = cookie;
            GlobalState.ObservableObject.CurrentCookie = cookie;
        }

        public static string GetCurrentCookie() => LocalSettings.Values[ApplicationSettingsKey.CurrentCookie]?.ToString();

        public static void RemoveCurrentCookie() => LocalSettings.Values.Remove(ApplicationSettingsKey.CurrentCookie);

        public static void AddCookie(AnoBbsCookie cookie)
        {
            if (LocalSettings.Values[ApplicationSettingsKey.AllCookies] is not ApplicationDataCompositeValue cookieListComposite)
            {
                cookieListComposite = new ApplicationDataCompositeValue();
            }

            if (cookieListComposite.ContainsKey(cookie.Name))
            {
                return;
            }

            cookieListComposite[cookie.Name] = cookie.Cookie;

            LocalSettings.Values[ApplicationSettingsKey.AllCookies] = cookieListComposite;
        }

        public static void DeleteCookie(string cookieName)
        {
            if (LocalSettings.Values[ApplicationSettingsKey.AllCookies] is not ApplicationDataCompositeValue cookieListComposite)
            {
                return;
            }

            cookieListComposite.Remove(cookieName);
            LocalSettings.Values[ApplicationSettingsKey.AllCookies] = cookieListComposite;
        }

        public static void LoadAllCookies()
        {
            if (LocalSettings.Values[ApplicationSettingsKey.AllCookies] is not ApplicationDataCompositeValue cookieListComposite)
            {
                return;
            }

            var parsedCookies = cookieListComposite.Select(cookie => new AnoBbsCookie
                {
                    Name = cookie.Key,
                    Cookie = cookie.Value.ToString()
                })
                .ToList();

            parsedCookies.ForEach(cookie => GlobalState.Cookies.Add(cookie));
        }

        public static void SetDarkThemeEnabled(bool isDarkThemeEnabled) => LocalSettings.Values[ApplicationSettingsKey.DarkThemeSelected] = isDarkThemeEnabled;

        public static bool IsDarkThemeEnabled() => LocalSettings.Values.ContainsKey(ApplicationSettingsKey.DarkThemeSelected) && (bool)LocalSettings.Values[ApplicationSettingsKey.DarkThemeSelected];

        public static void SetSubscriptionId(string subscriptionId) => LocalSettings.Values[ApplicationSettingsKey.SubscriptionId] = subscriptionId;

        public static string GetSubscriptionId() =>
            LocalSettings.Values.ContainsKey(ApplicationSettingsKey.SubscriptionId)
                ? LocalSettings.Values[ApplicationSettingsKey.SubscriptionId].ToString()
                : null;
    }
}
