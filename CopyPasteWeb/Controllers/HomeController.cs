using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CopyPastWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.UrlNewSession = Url.Action("NewSession");
            ViewBag.UrlSession = Url.Action("Session");

            return View();
        }

        public ActionResult NewSession()
        {
            return Content(Guid.NewGuid().ToString("N"));
        }

        public ActionResult Session(string id)
        {
            ViewBag.UrlHome = Url.Action("Index");
            ViewBag.SessionID = id;

            return View();
        }

        public ActionResult Send(string content)
        {

        }

        public ActionResult Receive()
        {

        }
    }
}