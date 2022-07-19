using Newtonsoft.Json;



namespace XoW.Models
{
    public class ThreadSubscription : ForumThread
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

#pragma warning disable CS0108
        // 隐藏 ForumThread 类中的 ReplyCount 属性
        // 因为在这里需要重新指定 JsonProperty
        [JsonProperty("reply_count")]
        public int ReplyCount { get; set; }
#pragma warning restore CS0108

        [JsonProperty("recent_replies")]
        public string RecentReplies { get; set; }

        public string Category { get; set; }

        [JsonProperty("file_id")]
        public string FileId { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string Po { get; set; }
    }
}
