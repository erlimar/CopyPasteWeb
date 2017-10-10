﻿using System.Web.Mvc;

namespace CopyPastWeb.Controllers
{
    using CopyPasteData;

    public class HomeController : Controller
    {
        public const string SESSION_ACTION_NAME = "Session";
        private readonly IDatasetRepository _db;

        public HomeController()
        {
            _db = new DatasetRepository();
        }

        public ActionResult Index()
        {
            ViewBag.UrlNewSession = Url.Action(nameof(NewSession));
            ViewBag.UrlSession = Url.Action(SESSION_ACTION_NAME);

            return View();
        }

        public ActionResult NewSession()
        {
            return Content(_db.NewID());
        }

        [ActionName(SESSION_ACTION_NAME)]
        public ActionResult SessionAction(string id)
        {
            ViewBag.UrlHome = Url.Action(nameof(Index));
            ViewBag.UrlSend = Url.Action(nameof(Send));
            ViewBag.UrlReceive = Url.Action(nameof(Receive));
            ViewBag.SessionID = id;

            return View();
        }

        public ActionResult Send(string content, string sessionID)
        {
            return Content(_db.SetValue(sessionID, content));
        }

        public ActionResult Receive(string sessionID)
        {
            return Content(_db.GetValue(sessionID));
        }
    }
}