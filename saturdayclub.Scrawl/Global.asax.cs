using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace saturdayclub.Scrawl
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class SaturdayClubScrawlApplication : System.Web.HttpApplication
    {
        private readonly IDictionary<string, string> commonTranslateTable = new Dictionary<string, string>();

        public SaturdayClubScrawlApplication()
        {
            this.commonTranslateTable[@"梅子"] = @"小妖";
        }

        public string TranslateToCommonWord(string word)
        {
            string commonWord = string.Empty;
            if (!this.commonTranslateTable.TryGetValue(word, out commonWord))
            {
                return word;
            }
            return commonWord;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            CreateCacheEntry();
        }

        private void CreateCacheEntry()
        {
            if (HttpRuntime.Cache["Scrawl"] != null)
                return;
            Scrawl();
            HttpRuntime.Cache.Add(
                "Scrawl",
                DateTime.UtcNow,
                null,
                DateTime.Now.AddMinutes(2),
                System.Web.Caching.Cache.NoSlidingExpiration,
                System.Web.Caching.CacheItemPriority.NotRemovable,
                (key, obj, reason) =>
                {
                    if (reason == System.Web.Caching.CacheItemRemovedReason.Expired &&
                        (string.Compare("Scrawl", key, false) == 0))
                    {
                        CreateCacheEntry();
                    }
                });
        }

        private void Scrawl()
        {
            string replyMsg = string.Empty;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.GetEncoding("GBK");
                    replyMsg = wc.DownloadString("http://www.niwota.com/quan/13142806");
                    //replyMsg = Server.HtmlEncode(replyMsg);

                    List<string> activityList = new List<string>();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(replyMsg);
                    var root = doc.GetElementbyId("activities");
                    var table = root.SelectNodes("//div[@class='col_body']/ul[@class='activities txt_light']/li[@class!='head']");
                    foreach (var col in table)
                    {
                        HtmlNode temp = HtmlNode.CreateNode(col.OuterHtml);
                        var node = temp.SelectSingleNode("//li/span[@class='theme']/a");
                        string title = node.InnerText.Trim();
                        node = temp.SelectSingleNode("//li/span[@class='host']/a");
                        string contact = TranslateToCommonWord(node.InnerText.Trim());
                        if (!string.IsNullOrEmpty(title))
                        {
                            activityList.Add(title + @" (联系人: " + contact + ")");
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < activityList.Count; i++)
                    {
                        sb.AppendLine((i + 1) + ". " + activityList[i]);
                    }
                    replyMsg = sb.ToString();
                }
            }
            catch
            {
                replyMsg = string.Empty;
            }

            if (!string.IsNullOrEmpty(replyMsg))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    try
                    {
                        XDocument xdoc = new XDocument();
                        xdoc.Add(new XElement("xml"));
                        xdoc.Element("xml").Add(new XElement("content"));
                        xdoc.Element("xml").Element("content").Add(new XCData(replyMsg));
                        wc.UploadString("http://saturdayclub.apphb.com/cache", "POST", xdoc.ToString());
                    }
                    catch
                    { }
                }
            }
        }
    }
}