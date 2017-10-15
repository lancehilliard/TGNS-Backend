using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Core.Messaging;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            //var messagePusher = new PushbulletMessagePusher(ConfigurationManager.AppSettings["PushbulletEncodedAuthKey"]);
            //var messagePushLogger = new MessagePushLogger(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            //var pushSummary = messagePusher.Push("Welcome!", "Welcome to the TGNS Channel!");
            //messagePushLogger.Log("ns2", 160301, messagePusher.PlatformName, pushSummary.Input, pushSummary.Output, pushSummary.ResultCode, pushSummary.ResultDescription);

            //var serverAdminJsonCreator = new ServerAdminJsonCreator();
            //var serverAdminJson = serverAdminJsonCreator.Create();
            //Console.WriteLine(serverAdminJson);

            return View();
        }

        // http://www.danielroot.info/2013/01/serve-up-html5-cache-manifest-using.html
        //public ActionResult Manifest()
        //{
        //    var manifestResult = new ManifestResult("1.0")
        //    {
        //        CacheResources = new List<string>()
        //     {
        //           Url.Action("Index", "ServerInfo"),
        //           "/content/site.css",
        //           "/scripts/main.js"
        //     },
        //        // NetworkResources = new string[] { Url.Action("Status", "Service") },
        //        FallbackResources = { { "/logo.png", "/logo_offline.png" } }
        //    };
        //    return manifestResult;
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}