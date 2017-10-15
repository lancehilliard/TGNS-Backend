using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace TGNS.Portal.Controllers
{
    public class ReadingController : Controller
    {
        //
        // GET: /Reading/
        public ActionResult Required()
        {
            return View();
        }

        public ActionResult RequiredSource()
        {
            var webClient = new WebClient();
            var result = webClient.DownloadString("https://docs.google.com/document/d/1tZ0ZcqnMnfklz6QPp5QwLqPuJDxaxp8U5IIh7HDN19k/pub?embedded=true");
            return Content(result);
        }
    }
}