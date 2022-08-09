using System;
using Windows.Storage;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace XoW.Services
{
    public static class ConfigurationManager
    {
        private const string ConfigFileName = "AppConfig.xml";

        public static async void LoadAppConfig()
        {
            var configFileUri = new Uri($"ms-appx:///{ConfigFileName}");
            var configFile = await StorageFile.GetFileFromApplicationUriAsync(configFileUri);
            var configFileXmlDocument = await XmlDocument.LoadFromFileAsync(configFile);

            LoadAiFaDianToken(configFileXmlDocument);
        }

        private static void LoadAiFaDianToken(XmlDocument rootDocument)
        {
            var aiFaDianSectionNode = rootDocument.DocumentElement.SelectSingleNode("./aifadian");
            if (aiFaDianSectionNode == null)
            {
                return;
            }

            var aiFaDianUsernameNode = aiFaDianSectionNode.SelectSingleNode("./add[@key='userId']/@value");
            var aiFaDianTokenNode = aiFaDianSectionNode.SelectSingleNode("./add[@key='token']/@value");
            GlobalState.AiFaDianUsername = aiFaDianUsernameNode?.NodeValue.ToString() ?? String.Empty;
            GlobalState.AiFaDianApiToken = aiFaDianTokenNode?.NodeValue.ToString() ?? String.Empty;
        }
    }
}
