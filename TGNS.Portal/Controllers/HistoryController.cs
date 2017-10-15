using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class HistoryController : AuthenticatedController
    {
        readonly IPlaypalsGetter _playpalsGetter;

        public HistoryController()
        {
             _playpalsGetter = new PlaypalsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        // GET: History
        public ActionResult Index()
        {
            var recentPlaypals = _playpalsGetter.GetRecent(PlayerId);
            return View(new HistoryIndexViewModel {RecentPlaypals = recentPlaypals});
        }
    }

    public class HistoryIndexViewModel
    {
        public IEnumerable<IPlayer> RecentPlaypals { get; set; }
    }
}