using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XoW.Models
{
    /// <summary>
    /// 串，也可以是回复，因为回复也是串
    /// </summary>
    public class ForumThread
    {
        /// <summary>
        /// 串ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 板块ID
        /// </summary>
        public string FId { get; set; }

        public int ReplyCount { get; set; }

        /// <summary>
        /// 图片相对地址
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 图片后缀
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// 发言时间
        /// </summary>
        public string Now { get; set; }

        /// <summary>
        /// 饼干
        /// </summary>
        [JsonProperty("user_hash")]
        public string UserHash { get; set; }

        /// <summary>
        /// 发言者名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 串标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 串内容，HTML
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否sage
        /// </summary>
        public string Sage { get; set; }

        /// <summary>
        /// 是否为炫酷红名
        /// </summary>
        public string Admin { get; set; }
        public string Hide { get; set; }

        [JsonIgnore]
        public int RemainReplies { get; set; }

        [JsonIgnore]
        public List<string> ContentParts { get; set; }

        public static void SplittingContentIntoList(ForumThread thread)
        {
            var contentParts = thread.Content.Split(Environment.NewLine);
            thread.ContentParts = new List<string>(contentParts);
        }
    }
}
