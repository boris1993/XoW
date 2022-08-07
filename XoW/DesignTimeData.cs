using System.Collections.ObjectModel;
using XoW.Models;

namespace XoW
{
    public class DesignTimeData
    {
        public static readonly ObservableCollection<AiFaDianUser> AiFaDianSponsoredUsers = new ObservableCollection<AiFaDianUser>
        {
            new AiFaDianUser
            {
                UserId = "3524370d11e8ae8852540025c377",
                Name = "Hee",
                Avatar = "https://pic2.afdiancdn.com/user/27f7sss7/avatar/2d9659585fc4798068efbb652e56c08a.jpg",
            },

            new AiFaDianUser
            {
                UserId = "sfff",
                Name = "sfsf：十五种幸福（新版）",
                Avatar = "https://pic2.afdiancdn.com/user/sdfsfsf/avatar/c13b6125cbd9fbe7810c79256df1f5b2_w4032_h3024_s3215.jpeg",
            },
        };
    }
}
