using System.Configuration;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class BetterKnownAsController : AuthenticatedController
    {
        private readonly IBkaDataGetter _bkaDataGetter;

        public BetterKnownAsController()
        {
            _bkaDataGetter = new BkaDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var bkaData = _bkaDataGetter.Get(PlayerId);
            ViewData["bkaData"] = bkaData;
            return View();
        }
	}
}