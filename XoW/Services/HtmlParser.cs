using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private const string XPathBrNode = "br";
        private const string XPathBrNodeAnywhere = "//br";
        private const string XPathDivWithClassThreadsContent = "//div[@class='h-threads-content']";
        private const string AttributeStyle = "style";
        private const string AttributeStyleParamColor = "color";
        private const string AttributeStyleParamFontWeight = "font-weight";
        private const string XPathANode = "a";
        private const string AttributeHyperlink = "href";
        private const string EmailToScheme = "mailto:";

        private static readonly Regex ThreadReferenceRegex = new Regex(@"^>> *(No\.)*\d+$", RegexOptions.Compiled);

        public static async Task<List<TextBlock>> ParseHtmlIntoTextBlocks(string htmlString, bool textSelectionEnabled = false)
        {
            var rootHtmlDoc = new HtmlDocument();
            rootHtmlDoc.LoadHtml(htmlString);
            var firstTextNode = rootHtmlDoc.DocumentNode.SelectNodes(XPathTextNodeAnywhere).FirstOrDefault();

            var shouldBoldForAllTextBlocks = false;
            Color? textBlockGlobalColor = null;
            GetGlobalStyleForAllTextBlocks(firstTextNode, ref shouldBoldForAllTextBlocks, ref textBlockGlobalColor);

            var textBlocksForContents = new List<TextBlock>();
            var linesOfHtmlString = htmlString.Split(Environment.NewLine).ToList();

            // 一部分换行符不标准，只有\n或\r
            // 在这里单独处理，将这些带有非标准换行符的行也正确分割
            for (var i = 0; i < linesOfHtmlString.Count; i++)
            {
                var line = linesOfHtmlString[i];
                if (line.Contains(Environment.NewLine))
                {
                    continue;
                }

                if (line.Contains("\r"))
                {
                    var linesSeparated = line.Split("\r");
                    linesOfHtmlString.RemoveAt(i);

                    var insertLocation = i;
                    foreach (var newLine in linesSeparated)
                    {
                        linesOfHtmlString.Insert(insertLocation, newLine);
                        insertLocation++;
                    }
                    //FIXME: i = i + linesOfHtmlString.size
                }

                if (line.Contains("\n"))
                {
                    var linesSeparated = line.Split("\n");
                    linesOfHtmlString.RemoveAt(i);

                    var insertLocation = i;
                    foreach (var newLine in linesSeparated)
                    {
                        linesOfHtmlString.Insert(insertLocation, newLine);
                        insertLocation++;
                    }
                    //FIXME: i = i + linesOfHtmlString.size
                }
            }

            foreach (var line in linesOfHtmlString)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(line);

                // 仅删除每行文字末尾的换行符
                // 如果只有换行符，那么就保留
                // 以保证显示效果与网页一致
                if (htmlDoc.DocumentNode.ChildNodes.Count > 1 && htmlDoc.DocumentNode.Descendants("br").Any())
                {
                    foreach (var node in htmlDoc.DocumentNode.SelectNodes(XPathBrNode))
                    {
                        node.Remove();
                    }
                }

                var content = htmlDoc.DocumentNode.InnerText;
                var deEntitizeContent = HtmlEntity.DeEntitize(content.Trim());

                TextBlock textBlock;
                var threadReferenceRegexMatch = ThreadReferenceRegex.Match(deEntitizeContent);
                if (threadReferenceRegexMatch.Success)
                {
                    var contentForThisTextBlock = deEntitizeContent;

                    var referencedThreadId = deEntitizeContent.Replace(">", "").Replace("No.", "").Trim();
                    var referencedContent = await GetReferencedThread(referencedThreadId);
                    contentForThisTextBlock += $"\n{referencedContent}\n";

                    textBlock = ComponentsBuilder.CreateTextBlock(contentForThisTextBlock, Colors.DarkGreen, textSelectionEnabled: textSelectionEnabled);
                }
                else
                {
                    textBlock = ComponentsBuilder.CreateTextBlock(deEntitizeContent, textSelectionEnabled: textSelectionEnabled);
                }
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

        private static async Task<string> GetReferencedThread(string threadId)
        {
            var refPage = await AnoBbsApiClient.GetReferencedThreadById(threadId);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(refPage);

            if (htmlDoc.DocumentNode.ChildNodes.Count > 1 && htmlDoc.DocumentNode.Descendants("br").Any())
            {
                foreach (var node in htmlDoc.DocumentNode.SelectNodes(XPathBrNodeAnywhere))
                {
                    node.Remove();
                }
            }

            var refNodeDocument = htmlDoc.DocumentNode.SelectSingleNode(XPathDivWithClassThreadsContent);
            var innerText = HtmlEntity.DeEntitize(refNodeDocument.InnerText.Trim());

            return innerText;
        }
    }
}
