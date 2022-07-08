using System.Collections.Generic;
using System.Configuration;

namespace XoW
{
    public class ApplicationSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        public string userHash { get; set; }

        [ApplicationScopedSetting()]
        public Dictionary<string, double> weightedCdnUrl { get; set; }
    }
}
