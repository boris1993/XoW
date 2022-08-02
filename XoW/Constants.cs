using System.Collections.Generic;

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
        /// 获取时间线<br/>
        /// 参数page为分页页码
        /// </summary>
        public static string GetTimeline = $"{BaseUrl}/Api/Timeline";

        /// <summary>
        /// 获取板块中的串<br/>
        /// 参数id为板块ID<br/>
        /// 参数page为分页页码
        /// </summary>
        public static string GetThreads = $"{BaseUrl}/Api/showf";

        /// <summary>
        /// 获取串和回复<br/>
        /// 参数id为串号<br/>
        /// 参数page为分页页码
        /// </summary>
        public static string GetReplies = $"{BaseUrl}/Api/thread";

        /// <summary>
        /// 只看po<br/>
        /// 参数id为串号<br/>
        /// 参数page为分页页码
        /// </summary>
        public static string GetPoOnlyReplies = $"{BaseUrl}/Api/po";

        /// <summary>
        /// 获取订阅<br/>
        /// 参数uuid为订阅ID<br/>
        /// 参数page为分页页码
        /// </summary>
        public static string GetSubscription = $"{BaseUrl}/Api/feed";

        /// <summary>
        /// 添加订阅<br/>
        /// 参数uuid为订阅ID<br/>
        /// 参数tid为串ID
        /// </summary>
        public static string AddSubscription = $"{BaseUrl}/Api/addFeed";

        /// <summary>
        /// 删除订阅<br/>
        /// 参数uuid为订阅ID<br/>
        /// 参数tid为串ID
        /// </summary>
        public static string DeleteSubscription = $"{BaseUrl}/Api/delFeed";

        /// <summary>
        /// 发新串<br/>
        /// </summary>
        public static string CreateNewThread = $"{BaseUrl}/Home/Forum/doPostThread.html";
    }

    public static class RequestBodyParamName
    {
        public const string FId = "fid";
        public const string Username = "name";
        public const string EMail = "email";
        public const string Title = "title";
        public const string Content = "content";
        public const string Water = "water";
        public const string Image = "image";
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

        /// <summary>
        /// 颜文字列表
        /// key为名字
        /// value为实际的颜文字
        /// </summary>
        public static Dictionary<string, string> Emoticons = new Dictionary<string, string>
        {
            { "|∀ﾟ", "|∀ﾟ" },
            { "(´ﾟДﾟ`)", "(´ﾟДﾟ`)" },
            { "(;´Д`)", "(;´Д`)" },
            { "(｀･ω･)", "(｀･ω･)" },
            { "(=ﾟωﾟ)=", "(=ﾟωﾟ)=" },
            { "| ω・´)", "| ω・´)" },
            { "|-` )", "|-` )" },
            { "|д` )", "|д` )" },
            { "|ー` )", "|ー` )" },
            { "|∀` )", "|∀` )" },
            { "(つд⊂)", "(つд⊂)" },
            { "(ﾟДﾟ≡ﾟДﾟ)", "(ﾟДﾟ≡ﾟДﾟ)" },
            { "(＾o＾)ﾉ", "(＾o＾)ﾉ" },
            { "(|||ﾟДﾟ)", "(|||ﾟДﾟ)" },
            { "( ﾟ∀ﾟ)", "( ﾟ∀ﾟ)" },
            { "( ´∀`)", "( ´∀`)" },
            { "(*´∀`)", "(*´∀`)" },
            { "(*ﾟ∇ﾟ)", "(*ﾟ∇ﾟ)" },
            { "(*ﾟーﾟ)", "(*ﾟーﾟ)" },
            { "(　ﾟ 3ﾟ)", "(　ﾟ 3ﾟ)" },
            { "( ´ー`)", "( ´ー`)" },
            { "( ・_ゝ・)", "( ・_ゝ・)" },
            { "( ´_ゝ`)", "( ´_ゝ`)" },
            { "(*´д`)", "(*´д`)" },
            { "(・ー・)", "(・ー・)" },
            { "(・∀・)", "(・∀・)" },
            { "(ゝ∀･)", "(ゝ∀･)" },
            { "(〃∀〃)", "(〃∀〃)" },
            { "(*ﾟ∀ﾟ*)", "(*ﾟ∀ﾟ*)" },
            { "( ﾟ∀。)", "( ﾟ∀。)" },
            { "( `д´)", "( `д´)" },
            { "(`ε´ )", "(`ε´ )" },
            { "(`ヮ´ )", "(`ヮ´ )" },
            { "σ`∀´)", "σ`∀´)" },
            { "    ﾟ∀ﾟ)σ", "    ﾟ∀ﾟ)σ" },
            { "ﾟ ∀ﾟ)ノ", "ﾟ ∀ﾟ)ノ" },
            { "(╬ﾟдﾟ)", "(╬ﾟдﾟ)" },
            { "(|||ﾟдﾟ)", "(|||ﾟдﾟ)" },
            { "( ﾟдﾟ)", "( ﾟдﾟ)" },
            { "Σ( ﾟдﾟ)", "Σ( ﾟдﾟ)" },
            { "( ;ﾟдﾟ)", "( ;ﾟдﾟ)" },
            { "( ;´д`)", "( ;´д`)" },
            { "(　д ) ﾟ ﾟ", "(　д ) ﾟ ﾟ" },
            { "( ☉д⊙)", "( ☉д⊙)" },
            { "(((　ﾟдﾟ)))", "(((　ﾟдﾟ)))" },
            { "( ` ・´)", "( ` ・´)" },
            { "( ´д`)", "( ´д`)" },
            { "( -д-)", "( -д-)" },
            { "(&gt;д&lt;)", "(&gt;д&lt;)" },
            { "･ﾟ( ﾉд`ﾟ)", "･ﾟ( ﾉд`ﾟ)" },
            { "( TдT)", "( TдT)" },
            { "(￣∇￣)", "(￣∇￣)" },
            { "(￣3￣)", "(￣3￣)" },
            { "(￣ｰ￣)", "(￣ｰ￣)" },
            { "(￣ . ￣)", "(￣ . ￣)" },
            { "(￣皿￣)", "(￣皿￣)" },
            { "(￣艸￣)", "(￣艸￣)" },
            { "(￣︿￣)", "(￣︿￣)" },
            { "(￣︶￣)", "(￣︶￣)" },
            { "ヾ(´ωﾟ｀)", "ヾ(´ωﾟ｀)" },
            { "(*´ω`*)", "(*´ω`*)" },
            { "(・ω・)", "(・ω・)" },
            { "( ´・ω)", "( ´・ω)" },
            { "(｀・ω)", "(｀・ω)" },
            { "(´・ω・`)", "(´・ω・`)" },
            { "(`・ω・´)", "(`・ω・´)" },
            { "( `_っ´)", "( `_っ´)" },
            { "( `ー´)", "( `ー´)" },
            { "( ´_っ`)", "( ´_っ`)" },
            { "( ´ρ`)", "( ´ρ`)" },
            { "( ﾟωﾟ)", "( ﾟωﾟ)" },
            { "(oﾟωﾟo)", "(oﾟωﾟo)" },
            { "(　^ω^)", "(　^ω^)" },
            { "(｡◕∀◕｡)", "(｡◕∀◕｡)" },
            { "/( ◕‿‿◕ )\\", "/( ◕‿‿◕ )\\" },
            { "ヾ(´ε`ヾ)", "ヾ(´ε`ヾ)" },
            { "(ノﾟ∀ﾟ)ノ", "(ノﾟ∀ﾟ)ノ" },
            { "(σﾟдﾟ)σ", "(σﾟдﾟ)σ" },
            { "(σﾟ∀ﾟ)σ", "(σﾟ∀ﾟ)σ" },
            { "|дﾟ )", "|дﾟ )" },
            { "┃電柱┃", "┃電柱┃" },
            { "ﾟ(つд`ﾟ)", "ﾟ(つд`ﾟ)" },
            { "ﾟÅﾟ )　", "ﾟÅﾟ )　" },
            { "⊂彡☆))д`)", "⊂彡☆))д`)" },
            { "⊂彡☆))д´)", "⊂彡☆))д´)" },
            { "⊂彡☆))∀`)", "⊂彡☆))∀`)" },
            { "(´∀((☆ミつ", "(´∀((☆ミつ" },
            { "･ﾟ( ﾉヮ´ )", "･ﾟ( ﾉヮ´ )" },
            { "(ﾉ)`ω´(ヾ)", "(ﾉ)`ω´(ヾ)" },
            { "ᕕ( ᐛ )ᕗ", "ᕕ( ᐛ )ᕗ" },
            { "(　ˇωˇ)", "(　ˇωˇ)" },
            { "( ｣ﾟДﾟ)｣＜", "( ｣ﾟДﾟ)｣＜" },
            { "( ›´ω`‹ )", "( ›´ω`‹ )" },
            { "(;´ヮ`)7", "(;´ヮ`)7" },
            { "(`ゥ´ )", "(`ゥ´ )" },
            { "(`ᝫ´ )", "(`ᝫ´ )" },
            { "( ᑭ`д´)ᓀ))д´)ᑫ", "( ᑭ`д´)ᓀ))д´)ᑫ" },
            { "σ( ᑒ )", "σ( ᑒ )" },
            { "齐齐蛤尔", "(`ヮ´ )σ`∀´) ﾟ∀ﾟ)σ" },
            { "大嘘", @"吁~~~~　　rnm，退钱！
&nbsp;　　　/　　　/ 
(　ﾟ 3ﾟ) `ー´) `д´) `д´)" },
            { "防剧透", "[h] [/h]" },
            { "骰子", "[n]" },
            { "高级骰子", "[n,m]" },
        };
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

        public const string Notification = "提示";
        public const string Error = "错误";

        public const string SubscriptionIdLabel = "订阅ID";
        public const string GenerateSubscriptionId = "生成订阅ID";
        public const string SubscriptionRecommendation = "建议在不同设备间使用相同订阅ID，以同步收藏";
        public const string NewThreadCreatedSuccessfully = "发串大成功";

        public const string CreateThreadButtonTooltip = "创建新串";
        public const string CreateReplyButtonTooltip = "创建回复";
        public const string RefreshThreadButtonTooltip = "刷新";
        public const string AddToFavouritesButtonTooltip = "收藏";
        public const string SearchThreadButtonTooltip = "搜索串";
        public const string GotoThreadTooltip = "跳转到串";
        public const string PoOnlyTooltip = "只看PO";
        public const string DeleteSubscription = "删除订阅";
        public const string ReportThread = "举报该串";
        public const string Close = "关闭";
        public const string SaveScratch = "保存草稿";
        public const string InsertEmoticon = "插入颜文字";
        public const string InsertImage = "插入图片";
        public const string RemoveImage = "删除图片";
        public const string Watermark = "水印";
        public const string Username = "名称";
        public const string Email = "E-Mail";
        public const string Title = "标题";
        public const string Content = "正文";
        public const string ImagePreview = "图片附件预览";
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
        public const string ContentRequiredWhenNoImageAttached = "没有上传文件的时候，必须填写内容";
    }
}
