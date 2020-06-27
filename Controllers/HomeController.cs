using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using Icomm.NewsCrawl.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Icomm.NewsCrawl.Website.Controllers
{
    [Controller]
    public class HomeController : Controller
    {
        protected string webRoot;
        protected string strUrl;
        protected string pathLink = "javascript:";
        private readonly IConfiguration configuration;
        private readonly news_crawlContext entities;

        public HomeController(IConfiguration configuration, news_crawlContext entities)
        {
            this.configuration = configuration;
            this.entities = entities;
        }
        //public ActionResult Index(String url, String type)
        //{
        //    ViewBag.Title = "Home Page";

        //    ViewBag.url = url;
        //    ViewBag.type = type;
        //    return View("Crawl");
        //}

        public IActionResult Conf()
        {
            ViewBag.Title = "Configuration";

            return View();
        }

        public IActionResult Domain()
        {
            ViewBag.Country = entities.Country.ToList();
            ViewBag.Title = "Domain";

            return View();
        }

        //public ActionResult Data()
        //{
        //    ViewBag.Title = "Data Page";

        //    return View();
        //}

        //public ActionResult Config()
        //{
        //    ViewBag.Title = "Config";

        //    return View();
        //}

        public IActionResult Crawl(String url, String type, String javascript)
        {
            ViewBag.url = url;
            ViewBag.type = type;
            ViewBag.javascript = javascript;
            return View();
        }

        [HttpPost]
        public IActionResult JsonResult([FromBody]ObjectJson json)
        {
            var url = WebUtility.UrlDecode(json.url);
            MyWebClient client = new MyWebClient() { Encoding = Encoding.UTF8 };
            client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
            if (json.isID)
            {
                client.DownloadData(url);
                var mainUrl = client.ResponseUri.ToString();
                var regex = JsonConvert.DeserializeObject<JsonIDInput>(json.data);
                string item = "";
                try
                {
                    var regexMatch = Regex.Match(mainUrl, regex._xpath);
                    item = regexMatch.Groups[regex.group_number].Value;
                }
                catch (Exception) { }
                return Json(JsonConvert.SerializeObject(new JsonIDresult { url = mainUrl, id = item }, Formatting.Indented));
            }
            else
            {
                var baseUri = new Uri(url);
                var isScript = json.javascript;
                var config = StructuredDataConfig.ParseJsonString(json.data);
                var html = client.DownloadString(!isScript ? url : this.configuration.GetAppSetting("UrlSeleniumGetHtmlExcuteJavascript") + "?url=" + WebUtility.UrlEncode(url));
                HtmlDocument docc = new HtmlDocument();
                docc.LoadHtml(html);
                var urltmp = "";
                HtmlNodeCollection nodes = docc.DocumentNode.SelectNodes("//a");
                if (nodes != null)
                    foreach (HtmlNode node in nodes)
                    {
                        if ((node.Attributes["href"] != null) && (node.Attributes["href"].Value != ""))
                        {
                            try
                            {
                                urltmp = node.Attributes["href"].Value.Trim();
                                node.Attributes["href"].Value = new Uri(baseUri, urltmp).AbsoluteUri;
                            }
                            catch (Exception) { }
                        }
                    };
                html = docc.DocumentNode.InnerHtml;
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(html);
                var result = JsonConvert.SerializeObject(scrapingResults, Formatting.Indented);
                return Json(result);
            }
        }

        // Api lấy về html của 1 trang web sau khi xử lý lại. Xóa hết javascript trong trang. CHuyển hết các link động sang tĩnh
        public IActionResult Html(string url, bool javascript)
        {
            var urlDecode = WebUtility.UrlDecode(url);
            Uri uriResult;
            // Kiểm tra xem url có hợp lệ hay không
            bool valid = Uri.TryCreate(urlDecode, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (valid)
            {
                // Download html của trang web
                var baseUri = new Uri(new Uri(urlDecode).GetLeftPart(UriPartial.Authority));
                MyWebClient client = new MyWebClient();
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
                var result = client.DownloadString(urlDecode);
                // CHuyển đổi từ html string sang HtmlParser
                var parser = new HtmlParser();
                var document = parser.ParseDocument(result);

                //
                IHtmlCollection<IElement> menuItems;
                if (!javascript)
                {
                    // Xóa hết các thẻ script
                    menuItems = document.QuerySelectorAll("script, base");
                    foreach (IElement item in menuItems)
                    {
                        item.Remove();
                    }
                }

                // Bắt tất cả các element có thuộc tính href
                menuItems = document.QuerySelectorAll("[href]");
                var urltmp = "";
                foreach (IElement item in menuItems)
                {
                    try
                    {
                        // Chuyển link động sang link tĩnh
                        urltmp = item.GetAttribute("href");
                        item.SetAttribute("href", new Uri(baseUri, urltmp).AbsoluteUri);
                    }
                    catch (Exception) { }
                }
                // Bắt tất cả các element có thuộc tính src
                menuItems = document.QuerySelectorAll("[src]");
                foreach (IElement item in menuItems)
                {
                    try
                    {
                        // Chuyển link động sang link tĩnh
                        urltmp = item.GetAttribute("src");
                        item.SetAttribute("src", new Uri(baseUri, urltmp).AbsoluteUri);
                    }
                    catch (Exception) { }
                }
                // Bắt tất cả thẻ a trong trang
                menuItems = document.QuerySelectorAll("a");
                foreach (IElement item in menuItems)
                {
                    try
                    {
                        // Chuyển link động sang link tĩnh
                        urltmp = item.GetAttribute("href");
                        item.SetAttribute("href", "javascript:LinkInfo(\"" + new Uri(baseUri, urltmp).AbsoluteUri + "\");");
                    }
                    catch (Exception) { }
                }
                // Bổ sung thêm file css, js để xử lý thêm trong trang
                var element = document.CreateElement("link");
                element.SetAttribute("href", "/Content/myStyle.css");
                element.SetAttribute("rel", "stylesheet");
                document.Head.AppendChild(element);
                var body = document.Body;
                element = document.CreateElement("script");
                element.SetAttribute("src", "/Scripts/jquery-3.3.1.min.js");
                body.Append(element);
                var scriptcss = document.CreateElement("script");
                scriptcss.SetAttribute("src", "/Scripts/css-selector-generator.js");
                body.Append(scriptcss);
                var baseurl = client.ResponseUri.ToString();
                var scriptInBody = document.CreateElement("script");
                scriptInBody.InnerHtml = "var url = \"" + baseurl + "\";";
                body.Append(scriptInBody);
                var script = document.CreateElement("script");
                script.SetAttribute("src", "/Scripts/myScript.min.js");
                body.Append(script);
                return PartialView((object)document.DocumentElement.OuterHtml);
            }
            else
            {
                ViewBag.Data = urlDecode;
                return View("Error");
            }
        }


        // Hàm  để thay thế các link của thẻ a, img từ link động sang link tĩnh
        private void replacePath(HtmlAgilityPack.HtmlDocument doc, String urltmp)
        {
            var baseUri = new Uri(urltmp);

            var url = "";
            HtmlAgilityPack.HtmlNodeCollection nodes;
            // Lấy hết các phần tử link trong trang
            nodes = doc.DocumentNode.SelectNodes("//link");
            //Begin ancho
            //nodes = doc.DocumentNode.SelectNodes("//a");
            if (nodes != null)
                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                {
                    // Thay thế href từ động sang tĩnh
                    if ((node.Attributes["href"] != null) && (node.Attributes["href"].Value != ""))
                    {
                        url = node.Attributes["href"].Value.Trim();
                        node.Attributes["href"].Value = new Uri(baseUri, url).AbsoluteUri;
                        //var url1 = FixLink(url.Trim(), true);
                        //if (url != url1)
                        //    node.Attributes["href"].Value = node.Attributes["href"].Value.Replace(url, url1);
                    }
                };
            // Lấy hết các link ảnh
            nodes = doc.DocumentNode.SelectNodes("//img");
            if (nodes != null)
                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                {
                    //thay đổi src từ động sang tĩnh
                    if ((node.Attributes["src"] != null) && (node.Attributes["src"].Value != ""))
                    {
                        url = node.Attributes["src"].Value.Trim();
                        node.Attributes["src"].Value = new Uri(baseUri, url).AbsoluteUri;
                        //var url1 = FixLink(url.Trim(), true);
                        //if (url != url1)
                        //    node.Attributes["href"].Value = node.Attributes["href"].Value.Replace(url, url1);
                    }
                };

            //Lấy hết các thẻ a trong trang
            nodes = doc.DocumentNode.SelectNodes("//a");
            if (nodes != null)
                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                {
                    // thay đổi href từ động sang tĩnh
                    if ((node.Attributes["href"] != null) && (node.Attributes["href"].Value != ""))
                    {
                        url = node.Attributes["href"].Value.Trim();
                        //if (url != url1)
                        //    node.Attributes["href"].Value = node.Attributes["href"].Value.Replace(url, url1);
                        node.Attributes["href"].Value = "javascript:LinkInfo(&quot;" + new Uri(baseUri, url).AbsoluteUri + "&quot;);";
                    }
                };
            //End ancho
        }
        public static bool CheckURLValid(string strURL)
        {
            return Uri.IsWellFormedUriString(strURL, UriKind.RelativeOrAbsolute); ;
        }
        public class ObjectJson
        {
            public string url { get; set; }
            public string data { get; set; }
            public bool javascript { get; set; }
            public bool isID { get; set; }
        }

        public class JsonIDresult
        {
            public string url { get; set; }
            public string id { get; set; }
        }

        public class JsonIDInput
        {
            public string _xpath { get; set; }
            public int group_number { get; set; }
        }
    }

    class MyWebClient : WebClient
    {
        Uri _responseUri;
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AllowAutoRedirect = true;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Timeout = 60 * 1000;
            return request;
        }

        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            _responseUri = response.ResponseUri;
            return response;
        }
    }
}
