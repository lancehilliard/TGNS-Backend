using System.Configuration;
using System.Web.Mvc;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class AutoMaxFpsController : AuthenticatedController
    {
        private readonly IAutoMaxFpsSetter _autoMaxFpsSetter;
        private readonly IAutoMaxFpsGetter _autoMaxFpsGetter;

        public AutoMaxFpsController()
        {
            _autoMaxFpsSetter = new AutoMaxFpsSetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _autoMaxFpsGetter = new AutoMaxFpsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpPost]
        public ActionResult UpdateAutoMaxFps(int maxFps)
        {
            if (maxFps >= 30 && maxFps <= 200)
            {
                _autoMaxFpsSetter.Set(PlayerId, maxFps);
                TempData["Success"] = "Auto Max FPS updated successfully.";
            }
            else
            {
                TempData["Error"] = "Auto Max FPS must be between 30 and 200.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var maxFps = _autoMaxFpsGetter.Get(PlayerId);
            ViewBag.MaxFps = maxFps;
            return View();
        }
    }
}