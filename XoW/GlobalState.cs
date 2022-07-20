using System.Collections.Generic;
using System.Collections.ObjectModel;
using XoW.Models;

namespace XoW
{
    public static class GlobalState
    {
        /// <summary>
        /// 版名 -> (版ID, 权限级别)
        /// 目前已知权限级别2代表需要cookie
        /// </summary>
        public static readonly Dictionary<string, (string, string)> ForumAndIdLookup = new Dictionary<string, (string, string)>();
        public static string CurrentForumId = Constants.TimelineForumId;
        public static string CurrentThreadId = default;
        public static string CurrentThreadAuthorUserHash = default;
        public static string CdnUrl;

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
