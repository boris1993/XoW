using System.Collections.Generic;

namespace XoW.Models
{
    public class AiFaDianRequestBody
    {
        public string UserId { get; set; }
        public string Params { get; set; }
        public long Ts { get; set; }
        public string Sign { get; set; }
    }

    public class AiFaDianResponse<T>
    {
        /// <summary>
        /// Error Code
        /// </summary>
        public int Ec { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string Em { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; }
    }

    public class AiFaDianSponsor
    {
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public List<AiFaDianSponsorList> List { get; set; }
    }

    public class AiFaDianSponsorList
    {
        public List<AiFaDianSponsorPlan> SponsorPlans { get; set; }
        public AiFaDianSponsorPlan CurrentPlan { get; set; }
        public string AllSumAmount { get; set; }
        public long FirstPayTime { get; set; }
        public long LastPayTime { get; set; }
        public AiFaDianUser User { get; set; }
    }

    public class AiFaDianSponsorPlan
    {
        public string PlanId { get; set; }
        public int Rank { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Pic { get; set; }
        public string Desc { get; set; }
        public string Price { get; set; }
        public long UpdateTime { get; set; }
        public int PayMonth { get; set; }
        public string ShowPrice { get; set; }
        public int Independent { get; set; }
        public int Permanent { get; set; }
        public int CanBuyHide { get; set; }
        public int NeedAddress { get; set; }
        public int ProductType { get; set; }
        public int SaleLimitCount { get; set; }
        public bool NeedInviteCode { get; set; }
        public long ExpireTime { get; set; }
        public List<object> SkuProcessed { get; set; }
        public int RankType { get; set; }
    }

    public class AiFaDianUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }
}
