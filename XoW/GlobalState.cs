using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using XoW.Models;
using XoW.Utils;
using XoW.Views;

namespace XoW
{
    public static class GlobalState
    {
        /// <summary>
        ///     版名 -> (版ID, 权限级别)
        ///     目前已知权限级别2代表需要cookie
        /// </summary>
        public static Dictionary<string, (string forumId, string permissionLevel)> ForumAndIdLookup = default;

        public static string CurrentForumId = Constants.TimelineForumId;
        public static string CurrentThreadId = default;
        public static string CurrentThreadAuthorUserHash = default;
        public static string CdnUrl;

        public static string AiFaDianUsername;
        public static string AiFaDianApiToken;

        public static bool isPoOnly;

        public static MainPage MainPageObjectReference;
        public static LargeImageViewUserControl LargeImageViewObjectReference;

        public static readonly ObservableCollection<AnoBbsCookie> Cookies = new ObservableCollection<AnoBbsCookie>();
        public static readonly ObservableCollection<AiFaDianUser> AiFaDianSponsoredUsers = new ObservableCollection<AiFaDianUser>();

        public static ObservableObject ObservableObject = new ObservableObject
        {
            BackgroundAndBorderColorBrush = new SolidColorBrush(Colors.LightGray),
            ListViewBackgroundColorBrush = new SolidColorBrush(Colors.White),
            CurrentCookie = ApplicationConfigurationHelper.GetCurrentCookie(),
            SubscriptionId = ApplicationConfigurationHelper.GetSubscriptionId(),
            ForumName = string.Empty,
            ThreadId = string.Empty
        };
    }
}
