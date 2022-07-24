using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using XoW.Models;

namespace XoW
{
    public static class GlobalState
    {
        /// <summary>
        /// 版名 -> (版ID, 权限级别)
        /// 目前已知权限级别2代表需要cookie
        /// </summary>
        public static Dictionary<string, (string forumId, string permissionLevel)> ForumAndIdLookup = default;
        public static string CurrentForumId = Constants.TimelineForumId;
        public static string CurrentThreadId = default;
        public static string CurrentThreadAuthorUserHash = default;
        public static string CdnUrl;

        public static ObservableBackgroundAndBorderColor BackgroundAndBorderColor = new ObservableBackgroundAndBorderColor
        {
            ColorBrush = new SolidColorBrush(Colors.LightGray)
        };

        public static ObservableListViewBackgroundColor ListViewBackgroundColor = new ObservableListViewBackgroundColor
        {
            ColorBrush = new SolidColorBrush(Colors.White)
        };


        public static ObservableCollection<AnonBbsCookie> Cookies = new ObservableCollection<AnonBbsCookie>();

        public static ObservableCurrentCookie CurrentCookie =
            new ObservableCurrentCookie
            {
                CurrentCookie = ApplicationConfigurationHelper.GetCurrentCookie()
            };

        public static ObservableSubscriptionId SubscriptionId =
            new ObservableSubscriptionId
            {
                SubscriptionId = ApplicationConfigurationHelper.GetSubscriptionId()
            };

        public static ObservableForumName CurrentForumName = new ObservableForumName();

        public static ObservableCurrentThreadId CurrentThreadIdDisplay = new ObservableCurrentThreadId();
    }
}
