using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class SessionController : BaseController
    {
        public ActionResult Crawl()
        {
            ViewBag.Title = "Session";

            return View();
        }
        public ActionResult Guild()
        {
            ViewBag.Title = "Guild";

            return View();
        }
    }
}