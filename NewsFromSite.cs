using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BotAit
{
    internal class NewsFromSite
    {
        public static async Task<string> GetNews()
        {
            var url = "https://aitanapa.ru";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var id = "moreResult";
            var element = doc.GetElementbyId(id);
            var printedElements = new HashSet<string>();


            var childElements = element.Descendants();

            List<string> arrNews = new List<string>();

            string resultString = "";

            var className = "newsIntrotext";
            var className2 = "tileTitle";
            var className3 = "jsPopupLink";
            var className4 = "newsPic";

            foreach(var childElement in childElements)
            {
                int i = 0;

                var classElements = childElement.Descendants().Where(d => d.Attributes.Contains("class") && (
                    d.Attributes["class"].Value.Contains(className) ||
                    d.Attributes["class"].Value.Contains(className2) ||
                    d.Attributes["class"].Value.Contains(className3) ||
                    d.Attributes["class"].Value.Contains(className4)));

                foreach (var classElement in classElements)
                {
                    var value = classElement.InnerHtml;

                        if (classElement.Attributes["class"].Value.Contains(className3))
                        {
                            var hrefValue = classElement.Attributes["href"].Value;
                            if (i == 0)
                            {
                                i++;
                                arrNews.Add(hrefValue + " " + "|" + " ");
                            }
                        }
                        else if (classElement.Attributes["class"].Value.Contains(className4))
                        {
                            var hrefValue = classElement.Attributes["style"].Value;
                            Match match = Regex.Match(hrefValue, @"background-image:url\('([^']*)'\)");
                            string backgroundImage = match.Groups[1].Value;
                            if (i == 1)
                            {
                                i++;
                                arrNews.Add(backgroundImage + " " + "|" + " ");
                            }
                        }
                        else if (classElement.Attributes["class"].Value.Contains(className2))
                        {
                            if (i == 2)
                            {
                                i++;
                                arrNews.Add(value + " " + "|" + " ");
                            }
                        }
                        else
                        {
                            if (i == 3)
                            {
                                i++;
                                arrNews.Add(value.Trim() + "#");
                                
                            }
                        }
                    
                }
            }

            resultString = string.Join("", arrNews);
            return resultString;
        }
    }
}

