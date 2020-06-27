using Icomm.NewsCrawl.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Icomm.NewsCrawl.Website.Controllers
{
    [Controller]
    public class ConfigController : ControllerBase
    {
        private readonly news_crawlContext entities;

        public ConfigController(news_crawlContext entities)
        {
            this.entities = entities;
        }


        #region Bổ sung thêm cấu hình domain
        [Route("api/domain")]
        [HttpPut]
        public String add([FromForm]Domain domain)
        {
            try
            {
                {
                    // Kiểm tra xem đã tồn tại hay chưa
                    List<Domain> domains = entities.Domain.Where(d => d.Domain1 == domain.Domain1 && d.Type == domain.Type).ToList();
                    if (domains.Count == 0)
                    {
                        entities.Domain.Add(domain);
                        entities.SaveChanges();
                        return "success";
                    }
                    else
                        return "Domain đã tồn tại";
                }
            }
            catch (Exception ex)
            {
                //return "Thêm Domain thất bại";
                throw new ArgumentException("Thêm Domain thất bại", ex);
            }
        }
        #endregion



        #region Lấy ra cấu hình domain
        // url: url của bài báo
        // type: Loại của cấu hình. 0: Chi tiết, 1: Cấu hình danh sách link chi tiết trong trang category, 2: Cấu hình link danh sách chủ đề trong domain, 3: Cấu hình lấy id của bài báo(nếu có)
        [Route("api/get")]
        [HttpGet]
        public Domain get(String url, int type)
        {

            try
            {
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
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion



        #region Chỉnh sửa cấu hình domain
        [Route("api/save")]
        [HttpPost]
        public Boolean save(Domain domain)
        {
            try
            {
                {
                    Domain domaintmp = entities.Domain.Find(domain.Id);
                    if (domaintmp != null && domain != null)
                    {
                        domaintmp.Id = domain.Id;
                        domaintmp.Domain1 = domain.Domain1;
                        domaintmp.Type = domain.Type;
                        domaintmp.Content = domain.Content;
                        entities.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //api Chỉnh sửa cấu hình domain
        [Route("api/update")]
        [HttpPost]
        public Boolean update(Domain domain)
        {
            try
            {
                {
                    Domain domaintmp = entities.Domain.Find(domain.Id);
                    if (domaintmp != null && domain != null)
                    {
                        domaintmp.Content = domain.Content;
                        domaintmp.Properties = domain.Properties;
                        entities.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "update");
                return false;
            }
        }
        #endregion



        // api xóa domain
        [Route("api/domain/delete/{id:int}")]
        [HttpDelete]
        public Result domain_delete(int id)
        {
            try
            {
                {
                    var config = entities.Domain.Find(id);
                    if (config != null)
                    {
                        entities.Domain.Remove(config);
                        entities.SaveChanges();
                        return new Result(true, config.Type);
                    }
                    else
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        // Api lấy dữ liệu của domain. Phân trang theo data table
        [Route("api/data")]
        [HttpPost]
        public DTResult<Domain> data([FromBody]DTParameters param)
        {
            try
            {
                List<Domain> data = new ResultSet(this.entities).GetResult(param.Order[0], param.Start, param.Length, param.Search.Value);
                int totalrecord;
                int count = new ResultSet(this.entities).Count(param.Search.Value, out totalrecord);
                DTResult<Domain> result = new DTResult<Domain>
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
                DTResult<Domain> result = new DTResult<Domain> { error = "Data error!" };
                return result;
            }
        }

        public class ResultSet
        {
            private readonly news_crawlContext entities;

            public ResultSet(news_crawlContext entities)
            {
                this.entities = entities;
            }
            public List<Domain> GetResult(DTOrder sort, int start, int length, string search)
            {
                {
                    search = search.ToLower();
                    var configs = from c in entities.Domain
                                  where
                                   (c.Id.ToString().Contains(search)
                                   || c.Domain1.ToString().Contains(search))
                                  select c;
                    if (sort.Column == 0 && sort.Dir == DTOrderDir.ASC)
                        return configs.OrderBy(s => s.Id).Skip(start).Take(length).ToList();
                    else if (sort.Column == 0 && sort.Dir == DTOrderDir.DESC)
                        return configs.OrderByDescending(s => s.Id).Skip(start).Take(length).ToList();
                    else if (sort.Column == 1 && sort.Dir == DTOrderDir.ASC)
                        return configs.OrderBy(s => s.Domain1).Skip(start).Take(length).ToList();
                    else if (sort.Column == 1 && sort.Dir == DTOrderDir.DESC)
                        return configs.OrderByDescending(s => s.Domain1).Skip(start).Take(length).ToList();
                    else if (sort.Column == 2 && sort.Dir == DTOrderDir.ASC)
                        return configs.OrderBy(s => s.Type).Skip(start).Take(length).ToList();
                    else
                        return configs.OrderByDescending(s => s.Type).Skip(start).Take(length).ToList();

                }
            }

            public int Count(string search, out int totalrecord)
            {
                {
                    totalrecord = entities.Domain.Count();
                    return entities.Domain
                    .Where(p => search == null
                    || p.Domain1.ToLower().Contains(search)).Count();
                }
            }
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
            public int Id { get; set; }
            public String Domain { get; set; }
            public String Status { get; set; }
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
    }
}
