namespace XoW
{
    public static class Url
    {
        /// <summary>
        /// X岛域名
        /// </summary>
        public const string BaseUrl = "https://www.nmbxd1.com";

        /// <summary>
        /// 获取CDN列表
        /// </summary>
        public static string GetCdn = $"{BaseUrl}/Api/getCdnPath";

        /// <summary>
        /// 获取板块列表
        /// </summary>
        public static string GetForums = $"{BaseUrl}/Api/getForumList";

        /// <summary>
        /// 获取时间线
        /// 参数page为分页页码
        /// </summary>
        public static string GetTimeline = $"{BaseUrl}/Api/Timeline";

        /// <summary>
        /// 获取板块中的串
        /// 参数id为板块ID
        /// 参数page为分页页码
        /// </summary>
        public static string GetThreads = $"{BaseUrl}/Api/showf";

        /// <summary>
        /// 获取串和回复
        /// 参数id为串号
        /// 参数page为分页页码
        /// </summary>
        public static string GetReplies = $"{BaseUrl}/Api/thread";
    }

    public static class Constants
    {
        public const string ForumName = "X岛匿名版";

        public const string UserAgent = "HavfunClient-UWP";

        public const string SettingsKeyCdn = "cdn";

        public const string TimelineForumId = "-1";
    }

    public static class TooltipContents
    {
        public const string CreateThreadButtonTooltipContent = "创建新串";
        public const string RefreshThreadButtonTooltipContent = "刷新";
    }
}
