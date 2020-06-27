using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;
using WebApplication3.Models;

namespace Icomm.NewsCrawl.Website.Controllers
{
    [ApiController]
    public class ApiSessionController : ControllerBase
    {
        [Route("session/session")]
        [HttpPut]
        public String add(session_smcc config)
        {

            try
            {
                using (var entities = new news_crawlEntities1())
                {
                    entities.session_smcc.Add(config);
                    entities.SaveChanges();
                    return "Thêm Domain thành công";
                }
            }
            catch (Exception)
            {
                return "Thêm Domain thất bại";
            }
        }
        //[System.Web.Http.Route("session/get")]
        //[System.Web.Http.HttpGet]
        //public Domain get(String sesstion, int status)
        //{

        //    try
        //    {
        //        using (var entities = new news_crawlEntities1())
        //        {
        //            Domain domainEntity = entities.session_smcc.Where(d => url.EndsWith(d.Domain1) && d.Type == type).FirstOrDefault();


        //            return domainEntity;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}

        [System.Web.Http.Route("session/save")]
        [System.Web.Http.HttpPost]
        public Boolean edit(session_smcc session)
        {
            try
            {
                using (var entities = new news_crawlEntities1())
                {
                    session_smcc sessiontmp = entities.session_smcc.Find(session.Id);
                    if (sessiontmp != null && session != null)
                    {
                        sessiontmp.session = session.session;
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

        [System.Web.Http.Route("session/delete/{id:int}")]
        [System.Web.Http.HttpDelete]
        public bool delete(int id)
        {
            try
            {
                using (var entities = new news_crawlEntities1())
                {
                    var config = entities.session_smcc.Find(id);
                    if (config != null)
                    {
                        entities.session_smcc.Remove(config);
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

        [System.Web.Http.Route("session/data")]
        [System.Web.Http.HttpPost]
        public DTResult<session_smcc> DataHandler(DTParameters param)
        {
            try
            {
                List<session_smcc> data = new ResultSet().GetResult(param.Order[0], param.Start, param.Length, param.Search.Value);
                int totalrecord;
                int count = new ResultSet().Count(param.Search.Value, out totalrecord);
                DTResult<session_smcc> result = new DTResult<session_smcc>
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
                DTResult<session_smcc> result = new DTResult<session_smcc> { error = "Data error!" };
                return result;
            }
        }

        public class ResultSet
        {
            public List<session_smcc> GetResult(DTOrder sort, int start, int length, string search)
            {
                using (var entities = new news_crawlEntities1())
                {
                    search = search.ToLower();
                    var configs = from c in entities.session_smcc
                                  where
                                   (c.Id.ToString().Contains(search)
                                   || c.session.ToString().Contains(search))
                                  select c;
                    if (sort.Column == 0 && sort.Dir == DTOrderDir.ASC)
                        return configs.OrderBy(s => s.Id).Skip(start).Take(length).ToList();
                    else if (sort.Column == 0 && sort.Dir == DTOrderDir.DESC)
                        return configs.OrderByDescending(s => s.Id).Skip(start).Take(length).ToList();
                    else if (sort.Column == 1 && sort.Dir == DTOrderDir.ASC)
                        return configs.OrderBy(s => s.session).Skip(start).Take(length).ToList();
                    else if (sort.Column == 1 && sort.Dir == DTOrderDir.DESC)
                        return configs.OrderByDescending(s => s.session).Skip(start).Take(length).ToList();
                    else if (sort.Column == 2 && sort.Dir == DTOrderDir.ASC)
                        return configs.OrderBy(s => s.status).Skip(start).Take(length).ToList();
                    else
                        return configs.OrderByDescending(s => s.status).Skip(start).Take(length).ToList();

                }
            }

            public int Count(string search, out int totalrecord)
            {
                using (var entities = new news_crawlEntities1())
                {
                    totalrecord = entities.session_smcc.Count();
                    return entities.session_smcc
                    .Where(p => search == null
                    || p.session.ToLower().Contains(search)).Count();
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
