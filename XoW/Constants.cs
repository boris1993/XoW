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

        public const string NoCookieSelected = "无生效的饼干";

        public const string TimelineForumId = "-1";

        public const string PermissionLevelCookieRequired = "2";

        /// <summary>
        /// 用于启动系统截图功能的URI<br/>
        /// 文档：<see href="https://docs.microsoft.com/zh-cn/windows/uwp/launch-resume/launch-screen-snipping"/>
        /// </summary>
        public const string SystemUriStartScreenClip = "ms-screenclip:edit?clippingMode=Rectangle";
    }

    public static class TooltipContents
    {
        public const string CreateThreadButtonTooltipContent = "创建新串";
        public const string CreateReplyButtonTooltipContent = "创建回复";
        public const string RefreshThreadButtonTooltipContent = "刷新";
        public const string AddToFavouritesButtonTooltipContent = "收藏";
        public const string SearchThreadButtonTooltipContent = "搜索串";
    }

    public static class ApplicationSettingsKey
    {
        public const string CurrentCookie = "current_cookie";
        public const string AllCookies = "all_cookies";
    }

    public static class ErrorMessage
    {
        public const string ScreenSnipFailed = "调用系统截图功能失败";
        public const string FileIsNotImage = "选择的文件不是图片";
        public const string QrCodeDecodeFailed = "饼干二维码解析失败";
    }
}
