using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using XoW.Utils;

namespace XoW.Services
{
    public static class HtmlParser
    {
        private const string XPathTextNodeAnywhere = "//text()";
        private const string XPathBrNodeAnywhere = "//br";
        private const string AttributeStyle = "style";
        private const string AttributeStyleParamColor = "color";
        private const string AttributeStyleParamFontWeight = "font-weight";
        private const string XPathANode = "a";
        private const string AttributeHyperlink = "href";
        private const string EmailToScheme = "mailto:";

        public static List<TextBlock> ParseHtmlIntoTextBlocks(string htmlString)
        {
            var rootHtmlDoc = new HtmlDocument();
            rootHtmlDoc.LoadHtml(htmlString);
            var firstTextNode = rootHtmlDoc.DocumentNode.SelectNodes(XPathTextNodeAnywhere).FirstOrDefault();

            bool shouldBoldForAllTextBlocks = false;
            Color? textBlockGlobalColor = null;
            GetGlobalStyleForAllTextBlocks(firstTextNode, ref shouldBoldForAllTextBlocks, ref textBlockGlobalColor);

            var textBlocksForContents = new List<TextBlock>();
            var linesOfHtmlString = htmlString.Split(Environment.NewLine);
            foreach (var line in linesOfHtmlString)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(line);

                if (htmlDoc.DocumentNode.Descendants("br").Any())
                {
                    foreach (var node in htmlDoc.DocumentNode.SelectNodes(XPathBrNodeAnywhere))
                    {
                        node.Remove();
                    }
                }

                var content = htmlDoc.DocumentNode.InnerText;
                var deEntitizeContent = HtmlEntity.DeEntitize(content.Trim());
                if (string.IsNullOrWhiteSpace(deEntitizeContent))
                {
                    continue;
                }

                var textBlock = ComponentsBuilder.CreateTextBlock(deEntitizeContent);
                SetEmailHyperLinkInTextBlock(htmlDoc.DocumentNode, textBlock);

                if (shouldBoldForAllTextBlocks)
                {
                    textBlock.FontWeight = FontWeights.Bold;
                }

                if (textBlockGlobalColor != null)
                {
                    textBlock.Foreground = new SolidColorBrush((Color)textBlockGlobalColor);
                }

                textBlocksForContents.Add(textBlock);
            }

            return textBlocksForContents;
        }

        private static void GetGlobalStyleForAllTextBlocks(HtmlNode parentNodeOfTheFirstTextNode, ref bool shouldBold, ref Color? textBlockGlobalColor)
        {
            // 先直接递归到顶层节点
            // 然后逐层在Span节点中寻找style属性
            // 接下来寻找color和font-weight
            if (parentNodeOfTheFirstTextNode.ParentNode != null)
            {
                GetGlobalStyleForAllTextBlocks(parentNodeOfTheFirstTextNode.ParentNode, ref shouldBold, ref textBlockGlobalColor);
            }

            var styleAttributeValue = parentNodeOfTheFirstTextNode.GetAttributeValue(AttributeStyle, null);
            if (styleAttributeValue == null)
            {
                return;
            }

            var attributeValues = styleAttributeValue.Split(";").Select(str => str.Trim()).ToList();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var styleParam in attributeValues)
            {
                var paramPair = styleParam.Split(":").Select(str => str.Trim()).ToList();
                switch (paramPair[0])
                {
                    case AttributeStyleParamColor:
                        textBlockGlobalColor = paramPair[1].FirstCharToUpper().ToColor();
                        break;
                    case AttributeStyleParamFontWeight:
                        shouldBold = paramPair[1].Equals("bold", StringComparison.OrdinalIgnoreCase);
                        break;
                }
            }
        }

        private static void SetEmailHyperLinkInTextBlock(HtmlNode textNode, TextBlock textBlock)
        {
            if (textNode.SelectSingleNode(XPathANode) == null)
            {
                return;
            }

            var hrefTarget = textNode.SelectSingleNode(XPathANode).GetAttributeValue(AttributeHyperlink, null);
            if (hrefTarget == null || !hrefTarget.StartsWith(EmailToScheme))
            {
                return;
            }

            var targetEmailAddress = hrefTarget.Split(EmailToScheme)[1];
            var textParts = textBlock.Text.Split(targetEmailAddress);

            textBlock.Text = "";
            foreach (var textPart in textParts)
            {
                if (!string.IsNullOrEmpty(textPart))
                {
                    textBlock.Inlines.Add(new Run
                    {
                        Text = textPart
                    });
                }
                else
                {
                    var hyperlink = new Hyperlink
                    {
                        NavigateUri = new Uri(hrefTarget)
                    };

                    hyperlink.Inlines.Add(new Run
                    {
                        Text = targetEmailAddress
                    });

                    textBlock.Inlines.Add(hyperlink);
                }
            }
        }
    }
}
