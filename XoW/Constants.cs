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

        /// <summary>
        /// 只看po
        /// 参数id为串号
        /// 参数page为分页页码
        /// </summary>
        public static string GetPoOnlyReplies = $"{BaseUrl}/Api/po";

        /// <summary>
        /// 获取订阅
        /// 参数uuid为订阅ID
        /// 参数page为分页页码
        /// </summary>
        public static string GetSubscription = $"{BaseUrl}/Api/feed";
    }

    public static class Constants
    {
        public const string ForumName = "X岛匿名版";

        public const string NoCookieSelected = "无生效的饼干";

        public const string TimelineForumId = "-1";

        public const string PermissionLevelCookieRequired = "2";

        public const string CookieNameUserHash = "userhash";

        public const string FavouriteThreadNavigationItemName = "收藏";

        public const string Po = "(PO)";

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
        public const string DarkThemeSelected = "dark_theme_selected";
        public const string SubscriptionId = "subscription_id";
    }

    public static class ComponentContent
    {
        public const string Ok = "知道了";
        public const string Confirm = "我确定！";
        public const string Cancel = "算了吧";

        public const string Error = "错误";

        public const string SubscriptionIdLabel = "订阅ID";
        public const string GenerateSubscriptionId = "生成订阅ID";
        public const string SubscriptionRecommendation = "建议在不同设备间使用相同订阅ID，以同步收藏";

        public const string CreateThreadButtonTooltip = "创建新串";
        public const string CreateReplyButtonTooltip = "创建回复";
        public const string RefreshThreadButtonTooltip = "刷新";
        public const string AddToFavouritesButtonTooltip = "收藏";
        public const string SearchThreadButtonTooltip = "搜索串";
        public const string GotoThreadTooltip = "跳转到串";
        public const string PoOnlyTooltip = "只看PO";
    }

    public static class ConfirmationMessage
    {
        public const string DeleteCookieConfirmation = "真的要删掉这块饼干吗？\n并不会真的碎饼哦，你随时还可以再添加进来~";
        public const string GenerateNewSubscriptionIdConfirmationTitle = "确定要生成新的订阅ID吗？";
        public const string GenerateNewSubscriptionIdConfirmationContent = "如果没有备份的话，你将会永久丢失当前的订阅ID！";
    }

    public static class ErrorMessage
    {
        public const string ScreenSnipFailed = "调用系统截图功能失败";
        public const string FileIsNotImage = "选择的文件不是图片";
        public const string QrCodeDecodeFailed = "饼干二维码解析失败";
        public const string CookieRequiredForThisForum = "浏览该板块需要拥有饼干";
        public const string SubscriptionIdRequiredForGettingSubscription = "需要有订阅ID才能获取订阅";
    }
}
