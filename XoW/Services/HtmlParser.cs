using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace XoW.Services
{
    public static class HtmlParser
    {
        private const string AttributeHyperlink = "href";
        private const string XPathBrNode = "//br";

        public static List<TextBlock> ParseHtmlIntoTextBlocks(string htmlString)
        {
            var textBlocksForContents = new List<TextBlock>();
            var linesOfHtmlString = htmlString.Split(Environment.NewLine);

            foreach (var line in linesOfHtmlString)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(line);

                if (htmlDoc.DocumentNode.Descendants("br").Any())
                {
                    foreach (var node in htmlDoc.DocumentNode.SelectNodes(XPathBrNode))
                    {
                        node.Remove();
                    }
                }

                var content = htmlDoc.DocumentNode.InnerText;
                var textBlock = ComponentsBuilder.CreateTextBlock(HtmlEntity.DeEntitize(content.Trim()));
                textBlocksForContents.Add(textBlock);
            }

            return textBlocksForContents;
        }
    }
}
