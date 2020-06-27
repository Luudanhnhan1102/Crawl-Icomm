using HtmlAgilityPack;
using Icomm.NewsCrawl.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenScraping;
using OpenScraping.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text;

namespace Icomm.NewsCrawl.Website.Controllers
{
    // Api quản lý các danh sách domain được thêm vào bảng start_url và url_crawl_list
    // Các url được thêm vào start_url là link trang chủ của domain. Giúp để lấy ra được danh sách các chủ đề có trong domain đó
    // bảng url_crawl_list là bảng danh sách các link chủ đề của các domain. Bảng này dùng để lập lịch định kỳ đẩy lên queue để bóc tách ra bài viết
    [Controller]
    public class DomainController : ControllerBase
    {
        private readonly news_crawlContext entities;

        public DomainController(news_crawlContext entities)
        {
            this.entities = entities;
        }
        // Thêm danh sách start url vào bảng start_url
        [Route("url/add")]
        [HttpPut]
        public ResultReturn add([FromForm]start_url listUrl)
        {

            try
            {
                    // Cắt chuỗi nhập vào ra array url
                    string[] lines = listUrl.url.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                    );
                    var urls = new Dictionary<string, string>();
                    foreach (var line in lines)
                    {
                        //Kiểm tra xem url có hợp lệ hay không
                        var url = validate(line).Replace(" ", "");
                        if (Uri.TryCreate(url, UriKind.Absolute, out Uri result))
                        {
                            var domain = result.Authority;
                            domain = domain.StartsWith("www.") ? domain.Substring(4) : domain;
                            if (!urls.ContainsKey(domain))
                                urls.Add(domain, url);
                        }
                        //urls.Add(url);
                    }
                    if (urls.Count > 0)
                    {
                        // Đẩy dữ liệu vào bảng start_url
                        List<start_url> startUrls = new List<start_url>() { };
                        foreach (KeyValuePair<string, string> entry in urls)
                        {
                            startUrls.Add(new start_url { url = entry.Value, domain = entry.Key });
                        }
                        entities.Database.ExecuteSqlCommand(new RawSqlString("usp_start_url_addList"), StartUrlParameters("@startUrl", startUrls));
                        //entities.Database.ExecuteSqlRaw("usp_start_url_addList", StartUrlParameters("@startUrl", startUrls));

                        return new ResultReturn("Thêm thành công " + urls.Count + " url!", 1);
                    }
                    return new ResultReturn("Không có url hợp lệ!", -1);
            }
            catch (Exception ex)
            {
                return new ResultReturn("Lỗi trong quá trình thêm", -1);
            }
        }
        // Lấy ra danh sách cấu hình của domain
        [Route("url/get")]
        [HttpGet]
        public Domain get(String url, int type)
        {

            try
            {
                    List<Domain> domainEntitys = entities.Domain.Where(d => (url == d.Domain1 || url.EndsWith("." + d.Domain1)) && d.Type == type).ToList();

                    if (domainEntitys.Count > 0)
                    {
                        foreach (Domain domain in domainEntitys)
                        {
                            if (domain.Domain1 == url)
                                return domain;
                        }
                        return domainEntitys.FirstOrDefault();
                    }
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        // Cập nhật lại link của domain 
        [Route("url/update")]
        [HttpPost]
        public ResultReturn update([FromForm]start_url start_url)
        {
            try
            {
                    start_url start_urltmp = entities.start_url.Find(start_url.ID);
                    if (start_urltmp != null && start_url != null)
                    {
                        var url = validate(start_url.url).Replace(" ", "");
                        var isDomain = Uri.TryCreate(url, UriKind.Absolute, out Uri result);
                        if (isDomain)
                        {
                            var domain = result.Authority.StartsWith("www.") ? result.Authority.Replace("www.", "") : result.Authority;
                            if (domain.Equals(start_urltmp.domain))
                            {
                                start_urltmp.url = url;
                                entities.SaveChanges();
                                return new ResultReturn("Cập nhật thành công!", 1);
                            }
                            return new ResultReturn("Không được thay đổi domain!", -1);
                        }
                        return new ResultReturn("Url không đúng", -1);
                    }
                    else
                        return new ResultReturn("Dữ liệu không tồn tại!", -1);
            }
            catch
            {
                return new ResultReturn("Lỗi trong quá trình xử lý!", -1);
            }
        }
        // Xóa link trong start_url
        [Route("url/delete")]
        [HttpPost]
        public ResultReturn delete([FromForm]int[] result)
        {
            try
            {
                {
                    var config = entities.start_url.Where(m => result.Contains(m.ID));
                    if (config != null)
                    {
                        entities.start_url.RemoveRange(config);
                        entities.SaveChanges();
                        return new ResultReturn("Xóa thành công!", 1);
                    }
                    else
                        return new ResultReturn("Dữ liệu không tồn tại!", -1);
                }
            }
            catch (Exception ex)
            {
                return new ResultReturn($"Xóa thất bại: {ex.ToString()}", -1);
            }
        }
        // Bóc tách danh sách các chủ đề có trong domain và thêm vào url_crawl_list
        [Route("url/push/list/category")]
        [HttpPost]
        public ResultReturn pushDataToCategory([FromForm]ResultList result1)
        {
            try
            {
                {
                    // Lấy ra danh sách url strong start_url cần bổ sung
                    var urls = entities.start_url.Where(m => result1.result.Contains(m.ID)).ToList();
                    if (urls != null)
                    {
                        var countUrl = 0;
                        MyWebClient client = new MyWebClient() { Encoding = Encoding.UTF8 };
                        client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";

                        foreach (var url1 in urls)
                        {

                            try
                            {
                                // Kiểm tra xem url có hợp lệ không
                                var urltmp = url1.url;
                                bool isUrl = Uri.TryCreate(urltmp, UriKind.Absolute, out Uri baseUri)
               && (baseUri.Scheme == Uri.UriSchemeHttp || baseUri.Scheme == Uri.UriSchemeHttps);
                                if (isUrl)
                                {
                                    var myDomain = baseUri.Authority;
                                    // Loại bỏ www ở đầu domain
                                    myDomain = myDomain.StartsWith("www.") ? myDomain.Substring(4) : myDomain;
                                    // lấy ra cấu hình của domain
                                    var domains = entities.Domain.Where(domain => (myDomain == domain.Domain1 || myDomain.EndsWith("." + domain.Domain1))
                                && domain.Type == 2).ToList();

                                    if (domains.Count > 0)
                                    {
                                        Domain domain = null;
                                        foreach (Domain domaintmp in domains)
                                        {
                                            if (domaintmp.Domain1 == myDomain)
                                            {
                                                domain = domaintmp;
                                                break;
                                            }
                                        }
                                        if (domain == null)
                                        {
                                            domain = domains.FirstOrDefault();
                                        }

                                        var data = client.DownloadData(urltmp);
                                        var contentType = client.ResponseHeaders["Content-Type"];
                                        // Kiểm tra xem nội dung của trang web có phải là html hay không
                                        if (contentType.StartsWith(@"text/"))
                                        {
                                            var jsonConfig = domain.Content;
                                            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                                            HtmlDocument docc = new HtmlDocument();
                                            var html = Encoding.UTF8.GetString(data);
                                            docc.LoadHtml(html);
                                            var url = "";
                                            // Chuyển link động trong trang web thành link tĩnh
                                            HtmlNodeCollection nodes = docc.DocumentNode.SelectNodes("//a");
                                            if (nodes != null)
                                                foreach (HtmlNode node in nodes)
                                                {
                                                    if ((node.Attributes["href"] != null) && (node.Attributes["href"].Value != ""))
                                                    {
                                                        try
                                                        {
                                                            url = node.Attributes["href"].Value.Trim();
                                                            node.Attributes["href"].Value = new Uri(baseUri, url).AbsoluteUri;
                                                        }
                                                        catch { }
                                                    }
                                                };
                                            html = docc.DocumentNode.InnerHtml;
                                            // Bóc tách ra danh sách chủ đề của domain dựa vào cấu hình
                                            var openScraping = new StructuredDataExtractor(config);
                                            var scrapingResults = openScraping.Extract(html);
                                            var result = JsonConvert.SerializeObject(scrapingResults, Formatting.Indented);
                                            // Lấy ra danh sách chủ đề trong trang báo
                                            if (scrapingResults.Count > 0)
                                            {
                                                var o = JObject.Parse(result);
                                                JToken token = o["link"];
                                                if (token != null)
                                                {
                                                    List<url_crawl_list> listCrawl = new List<url_crawl_list>() { };
                                                    int count = 0;
                                                    var lsturl = token is JArray ? ((JArray)token).Select(m => m?.ToString()?.Trim()).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList() : new List<string>() { token.ToString() };
                                                    foreach (var valuetmp in lsturl)
                                                    {
                                                        if (valuetmp.Contains(myDomain) && valuetmp.StartsWith("http"))
                                                        {
                                                            listCrawl.Add(new url_crawl_list { url = valuetmp, status = 1, domain = myDomain, interval = result1.interval, module = result1.module, schedule_time = DateTime.Now });
                                                            count++;
                                                        }
                                                    }
                                                    if (count > 0)
                                                    {
                                                        countUrl++;
                                                        // Thêm danh sách url chủ đề vào bảng url_crawl_list
                                                        //entities.Database.ExecuteSqlRaw("usp_url_crawl_list_addList"
                                                        //, UrlCrawlListParameters("@urlCrawlList", listCrawl));
                                                        entities.Database.ExecuteSqlCommand(new RawSqlString("usp_url_crawl_list_addList")
                                                            , UrlCrawlListParameters("@urlCrawlList", listCrawl));

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        if (countUrl > 0)
                            return new ResultReturn("Thêm thành công " + countUrl + " url !", 1);
                        return new ResultReturn("Không thêm được url nào, vui lòng xem lại cấu hình!", -1);
                    }
                    else
                        return new ResultReturn("Không tìm thấy bản ghi nào!", -1);
                }
            }
            catch
            {
                return new ResultReturn("Thêm thất bại!", -1);
            }
        }

        [Route("url/data")]
        [HttpPost]
        public DTResult<start_url> DataHandler([FromBody]DTParameters param)
        {
            try
            {
                List<start_url> data = new ResultSet(this.entities).GetResult(param.Order[0], param.Start, param.Length, param.Search.Value);
                int totalrecord;
                int count = new ResultSet(this.entities).Count(param.Search.Value, out totalrecord);
                DTResult<start_url> result = new DTResult<start_url>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = totalrecord
                };
                return result;
            }
            catch (Exception ex)
            {
                DTResult<start_url> result = new DTResult<start_url> { error = $"Data error: {ex.ToString()}" };
                return result;
            }
        }

        [Route("url/data/not/config")]
        [HttpPost]
        public DTResult<urlDTO> DataHandlerNotConfig([FromBody]DTParameters param)
        {
            try
            {
                List<urlDTO> data = new ResultSet(this.entities).GetResultCheckConfig(param.Order[0], param.Start, param.Length, param.Search.Value, 1, out int totalrecord, out int count);
                DTResult<urlDTO> result = new DTResult<urlDTO>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = totalrecord
                };
                return result;
            }
            catch (Exception)
            {
                DTResult<urlDTO> result = new DTResult<urlDTO> { error = "Data error!" };
                return result;
            }
        }


        [Route("url/data/not/push/to/list")]
        [HttpPost]
        public DTResult<urlDTO> DataHandlerNotPush([FromBody]DTParameters param)
        {
            try
            {
                List<urlDTO> data = new ResultSet(this.entities).GetResultCheckConfig(param.Order[0], param.Start, param.Length, param.Search.Value, 2, out int totalrecord, out int count);
                DTResult<urlDTO> result = new DTResult<urlDTO>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = totalrecord
                };
                return result;
            }
            catch (Exception)
            {
                DTResult<urlDTO> result = new DTResult<urlDTO> { error = "Data error!" };
                return result;
            }
        }


        private static string validate(string item)
        {
            while (item.IndexOfAny(new[] { '\t', '\r', '\n' }) != -1)
            {
                item = item.Replace('\t', ' ').Replace('\r', ' ')
                    .Replace('\n', ' ');
            }
            while (item.Contains("  "))
            {
                item = item.Replace("  ", " ");
            }
            return item.Normalize().Trim();
        }

        public class ResultId
        {
            public int[] result { get; set; }
        }

        public class ResultList
        {
            public int[] result { get; set; }
            public string module { get; set; }
            public int interval { get; set; }
        }

        public class ResultSet
        {
            private readonly news_crawlContext entities;

            public ResultSet(news_crawlContext entities)
            {
                this.entities = entities;
            }
            public List<start_url> GetResult(DTOrder sort, int start, int length, string search)
            {
                {
                    search = search.ToLower();
                    var configs = entities.start_url.Where(m => m.ID.ToString().Contains(search) || m.url.Contains(search));
                    string orderby = sort.Column == 1 ? "ID" : "url";
                    orderby += sort.Dir == DTOrderDir.DESC ? " DESC" : " ASC";
                    return configs.OrderBy(orderby).Skip(start).Take(length).ToList();
                }
            }

            public List<urlDTO> GetResultCheckConfig(DTOrder sort, int start, int length, string search, int type, out int totalRecord, out int count)
            {
                {
                    totalRecord = entities.start_url.Count();
                    search = search.ToLower();
                    string orderby = sort.Column == 1 ? "ID" : "url";
                    orderby += sort.Dir == DTOrderDir.DESC ? " DESC" : " ASC";

                    if (type == 1)
                    {
                        count = entities.url_not_config.Count();
                        return entities.url_not_config.Where(m => m.ID.ToString().Contains(search) || m.url.Contains(search)).OrderBy(orderby).Select(m => new urlDTO { ID = m.ID, domain = m.domain, url = m.url }).Skip(start).Take(length).ToList();
                    }
                    else if (type == 2)
                    {
                        count = entities.url_not_push_to_list.Count();
                        return entities.url_not_push_to_list.Where(m => m.ID.ToString().Contains(search) || m.url.Contains(search)).OrderBy(orderby).Select(m => new urlDTO { ID = m.ID, domain = m.domain, url = m.url }).Skip(start).Take(length).ToList();
                    }
                    count = 0;
                    return null;
                }
            }

            public int Count(string search, out int totalrecord)
            {
                {
                    totalrecord = entities.start_url.Count();
                    return entities.start_url
                    .Where(p => search == null
                    || p.url.ToLower().Contains(search) || p.ID.ToString().Contains(search)).Count();
                }
            }
        }

        public class ResultReturn
        {
            public ResultReturn(string messeage, int status)
            {
                result = messeage;
                this.status = status;
            }
            public string result { get; set; }
            public int status { get; set; }
        }

        public class Order
        {
            public Order(String column, DTOrderDir dir)
            {
                Column = column;
                Dir = dir;
            }
            public String Column { get; set; }

            public DTOrderDir Dir { get; set; }
        }
        public class ConfigShort
        {
            public int ID { get; set; }
            public String url { get; set; }
        }
        public class Result
        {
            public Result(bool success, int type)
            {
                this.success = success;
                this.type = type;
            }
            public bool success { get; set; }
            public int type { get; set; }

        }

        static SqlParameter StartUrlParameters(string name, List<start_url> graphUsers)
        {
            return new SqlParameter(name, StartUrlDataTable(graphUsers))
            {
                TypeName = "_startUrl",
                SqlDbType = SqlDbType.Structured
            };
        }

        private static DataTable StartUrlDataTable(List<start_url> graphUsers)
        {
            var dt = new DataTable();
            dt.Columns.Add("url");
            dt.Columns.Add("domain");
            foreach (var item in graphUsers)
            {
                var dr = dt.NewRow();
                dr["url"] = item.url;
                dr["domain"] = item.domain;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        static SqlParameter UrlCrawlListParameters(string name, List<url_crawl_list> graphUsers)
        {
            return new SqlParameter(name, UrlCrawlListDataTable(graphUsers))
            {
                TypeName = "_urlCrawlList",
                SqlDbType = SqlDbType.Structured
            };
        }

        private static DataTable UrlCrawlListDataTable(List<url_crawl_list> graphUsers)
        {
            var dt = new DataTable();
            dt.Columns.Add("url");
            dt.Columns.Add("interval");
            dt.Columns.Add("schedule_time", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("status");
            dt.Columns.Add("module");
            dt.Columns.Add("domain");
            foreach (var item in graphUsers)
            {
                var dr = dt.NewRow();
                dr["url"] = item.url;
                dr["interval"] = item.interval;
                dr["schedule_time"] = item.schedule_time;
                dr["status"] = item.status;
                dr["module"] = item.module;
                dr["domain"] = item.domain;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public class urlDTO
        {
            public int ID { get; set; }
            public string url { get; set; }
            public string domain { get; set; }
        }
    }
}
