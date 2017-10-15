using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class MyController : AuthenticatedController
    {
        private readonly FullSpecOptionsOptionsModelGetter _fullSpecOptionsModelGetter;
        private readonly BadgePlayerGetter _badgePlayerGetter;
        private readonly ITaglineGetter _taglineGetter;
        private readonly IApprovalsGetter _approvalsGetter;
        private readonly IAutoMaxFpsGetter _autoMaxFpsGetter;

        public MyController()
        {
            _fullSpecOptionsModelGetter = new FullSpecOptionsOptionsModelGetter();
            _badgePlayerGetter = new BadgePlayerGetter();
            _taglineGetter = new TaglineGetter();
            _approvalsGetter = new ApprovalsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _autoMaxFpsGetter = new AutoMaxFpsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpGet]
        public ActionResult Settings()
        {
            var fullSpecOptionsModel = _fullSpecOptionsModelGetter.Get();
            ViewBag.FullSpecOptedInStatus = fullSpecOptionsModel.EnrolledPlayerIds.Any(x => x == PlayerId) ? "Opted In" : "Opted Out";

            var showInGameBadge = _badgePlayerGetter.Get(PlayerId).SingleOrDefault(x=>x.ShowInGame);
            ViewBag.ShowInGameBadgeName = showInGameBadge != null ? showInGameBadge.DisplayName : "None";

            var tagline = _taglineGetter.Get(PlayerId);
            tagline = string.IsNullOrWhiteSpace(tagline) ? "None" : tagline;
            ViewBag.Tagline = tagline;
            ViewBag.Ns2Id = PlayerId;

            var maxFps = _autoMaxFpsGetter.Get(PlayerId);
            ViewBag.MaxFps = maxFps?.ToString() ?? "None";

            var recentApprovals = _approvalsGetter.GetByTargetPlayerId("ns2", PlayerId).Where(x=>x.Created > DateTime.Now.AddDays(-14)).OrderByDescending(x => x.Created);
            var recentApprovalsWithReasons = recentApprovals.Where(x => !string.IsNullOrWhiteSpace(x.Reason)).Take(10);
            ViewBag.RecentApprovals = recentApprovals;
            ViewBag.RecentApprovalsCount = recentApprovals.Count();
            ViewBag.RecentApprovalsWithReasons = recentApprovalsWithReasons;
            ViewBag.RecentApprovalsWithReasonsCount = recentApprovalsWithReasons.Count();

            return View();
        }
	}
}