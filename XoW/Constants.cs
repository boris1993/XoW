namespace XoW
{
    public static class Url
    {
        /// <summary>
        /// X岛域名
        /// </summary>
        public const string DomainName = "www.nmbxd1.com";

        public static string BaseUrl = $"https://{DomainName}";

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

        public const int PermissionLevelCookieRequired = 2;

        public const string CookieNameUserHash = "userhash";

        public const string FavouriteThreadNavigationItemName = "收藏";

        /// <summary>
        /// 用于启动系统截图功能的URI<br/>
        /// 文档：<see href="https://docs.microsoft.com/zh-cn/windows/uwp/launch-resume/launch-screen-snipping"/>
        /// </summary>
        public const string SystemUriStartScreenClip = "ms-screenclip:edit?clippingMode=Rectangle";
    }

    public static class ApplicationSettingsKey
    {
        public const string CurrentCookie = "current_cookie";
        public const string AllCookies = "all_cookies";
    }

    public static class ComponentContent
    {
        public const string Confirm = "我确定！";
        public const string Cancel = "算了吧";

        public const string CreateThreadButtonTooltip = "创建新串";
        public const string CreateReplyButtonTooltip = "创建回复";
        public const string RefreshThreadButtonTooltip = "刷新";
        public const string AddToFavouritesButtonTooltip = "收藏";
        public const string SearchThreadButtonTooltip = "搜索串";
    }

    public static class ConfirmationMessage
    {
        public const string DeleteCookieConfirmation = "真的要删掉这块饼干吗？\n并不会真的碎饼哦，你随时还可以再添加进来~";
    }

    public static class ErrorMessage
    {
        public const string ScreenSnipFailed = "调用系统截图功能失败";
        public const string FileIsNotImage = "选择的文件不是图片";
        public const string QrCodeDecodeFailed = "饼干二维码解析失败";
        public const string CookieRequiredForThisForum = "浏览该板块需要拥有饼干";
    }
}
