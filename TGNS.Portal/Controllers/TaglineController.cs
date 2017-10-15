using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class TaglineController : AuthenticatedController
    {
        private readonly ITaglineGetter _taglineGetter;
        private readonly ITaglineSetter _taglineSetter;
        private static readonly int TaglineMaxLength = 110;

        public TaglineController()
        {
            _taglineGetter = new TaglineGetter();
            _taglineSetter = new TaglineSetter();
        }

        [HttpGet]
        public ActionResult Manage()
        {
            ViewBag.Tagline = _taglineGetter.Get(PlayerId);
            return View();
        }

        [HttpPost]
        public ActionResult Manage(string tagline)
        {
            var length = tagline.Length <= TaglineMaxLength ? tagline.Length : TaglineMaxLength;
            _taglineSetter.Set(PlayerId, tagline.Substring(0, length));
            TempData["success"] = "Tagline updated.";
            return RedirectToAction("Manage", "Tagline");
        }
	}
}