using System.Collections.ObjectModel;
using XoW.Models;

namespace XoW
{
    public class DesignTimeData
    {
        public ObservableCollection<ForumThread> _sampleThreads { get; set; } = new ObservableCollection<ForumThread>
        {
            new ForumThread
            {
                Id = -1,
                FId = -1,
                ReplyCount = 258,
                Img = "2022-06-18/62acedc59ef24",
                Ext = ".png",
                Now = "2022-06-18(六)05:10:29",
                UserHash = "Admin",
                Name = "无名氏",
                Title = "无标题",
                Content = "&bull; 好久不见<br />\r\n&bull; 灾后重建有序进行中，由于目前人手不足，部分板块暂停开放；旧串三酱会开发相关功后能陆续恢复<br />\r\n&bull; 使用逻辑与旧岛相同，但版规理论上只禁止任何广义上法律不允许的内容。除此之外禁晒妹及恶臭现充话题<br />\r\n&bull; 本串为QA串，如对本岛未来发展/人员构成/先前事件 持有疑问请在本串留言，我们会积攒进行回应<br />\r\n&bull; 回应长文包含了大部分问题的解释，如对本岛及先前事件好奇请耐心阅读<br />\r\n&bull; 特殊事项请携缘由联系邮箱：<a href=mailtohelp@nmbxd.com>help@nmbxd.com</a><br />\r\n&bull;  原有数据会伴随新系统上线进行恢复，恢复前啥也没有，稍安勿躁<br />\r\n&bull; 特殊时期，冷却时间/板块分布/处理响应时间 均为特殊情况，会慢慢恢复<br />\r\n&bull; 如暂时无原需要板块，请将串发至综一或者讨论内容意思相近板块<br />\r\n&bull; <span style=\" font-weight: bold \"><font color=\"#CC0000\">不论是敌是友，久等了，欢迎回来。</font><span></h1>",
                Sage = 1,
                Admin = 1,
                Hide = 0,
                RemainReplies = 253,
            }
        };
    }
}
