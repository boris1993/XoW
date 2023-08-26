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
        internal static Dictionary<string, (string forumId, string permissionLevel)> ForumAndIdLookup = default;

        internal static string CurrentForumId = Constants.TimelineForumId;
        internal static string CurrentThreadId = default;
        internal static string CurrentThreadAuthorUserHash = default;
        internal static string CdnUrl;

        internal static string AiFaDianUsername;
        internal static string AiFaDianApiToken;

        internal static bool isPoOnly;

        internal static MainPage MainPageObjectReference;
        internal static LargeImageViewUserControl LargeImageViewObjectReference;

        internal static readonly ObservableCollection<AnoBbsCookie> Cookies = new ObservableCollection<AnoBbsCookie>();
        internal static readonly ObservableCollection<AiFaDianUser> AiFaDianSponsoredUsers = new ObservableCollection<AiFaDianUser>();

        internal static ObservableObject ObservableObject = new ObservableObject
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
