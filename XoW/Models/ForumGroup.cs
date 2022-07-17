using Newtonsoft.Json;
using System.Collections.Generic;

namespace XoW.Models
{
    /// <summary>
    /// 板块组，如“综合”，“二次元”
    /// </summary>
    public class ForumGroup
    {
        /// <summary>
        /// 该板块组的ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 服务器的排序值，越小优先级越高，若为-1则自动排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 板块组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 板块列表
        /// </summary>
        public IEnumerable<Forum> Forums { get; set; }

    }

    /// <summary>
    /// 板块，如“时间线”，“综合版1”
    /// </summary>
    public class Forum
    {
        /// <summary>
        /// 板块ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属板块组ID
        /// </summary>
        public string FGroup { get; set; }

        /// <summary>
        /// 服务器的排序值，越小优先级越高，若为-1则自动排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 板块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 板块显示的名字，若该值不为空则显示该值(包含html)
        /// </summary>
        public string ShowName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 该板块发言间隔
        /// </summary>
        public int Interval { get; set; }

        [JsonProperty("thread_count")]
        public int threadCount { get; set; }

        [JsonProperty("permission_level")]
        public string permissionLevel { get; set; }

        [JsonProperty("forum_fuse_id")]
        public string forumFuseId { get; set; }
        public string CreatedAt { get; set; }
        public string UpdateAt { get; set; }

        /// <summary>
        /// 始终为n
        /// </summary>
        public string Status { get; set; }

    }
}
