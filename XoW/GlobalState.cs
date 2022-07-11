using System.Collections.Generic;
using System.Collections.ObjectModel;
using XoW.Models;

namespace XoW
{
    public static class GlobalState
    {
        public static readonly Dictionary<string, int> ForumAndIdLookup = new Dictionary<string, int>();
        public static string CurrentForumId = Constants.TimelineForumId;
        public static string CurrentThreadId = "";
        public static string CdnUrl;
        public static string CurrentCookie = ApplicationConfigurationHelper.GetCurrentCookie();
        public static ObservableCollection<AnonBbsCookie> Cookies = new ObservableCollection<AnonBbsCookie>();
    }
}
