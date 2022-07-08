using System.Collections.Generic;

namespace XoW.Models
{
    public class ThreadReply : ForumThread
    {
        public List<ForumThread> Replies { get; set; }
    }
}
