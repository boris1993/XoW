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
        public static readonly Dictionary<string, (int, int)> ForumAndIdLookup = new Dictionary<string, (int forumId, int permissionLevel)>();
        public static string CurrentForumId = Constants.TimelineForumId;
        public static string CurrentThreadId = "";
        public static string CdnUrl;
        public static string CurrentCookie = ApplicationConfigurationHelper.GetCurrentCookie();
        public static ObservableCollection<AnonBbsCookie> Cookies = new ObservableCollection<AnonBbsCookie>();
    }
}
