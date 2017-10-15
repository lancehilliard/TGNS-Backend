using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class CaptainsEligibilityController : AdminsController
    {
        private readonly IBadgePlayersGetter _badgePlayersGetter;

        public CaptainsEligibilityController()
        {
            _badgePlayersGetter = new BadgePlayersGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
        }

        public ActionResult Eligibility()
        {
            var playersWhoHaveEarnedSilverCompetitorBadge = _badgePlayersGetter.Get(2).OrderBy(x=>x.Name).ThenBy(x=>x.PlayerId);
            var playersWhoHaveEarnedBronzeWarriorBadge = _badgePlayersGetter.Get(25).OrderBy(x => x.Name).ThenBy(x => x.PlayerId);
            return View(new CaptainsEligibleViewModel{ PlayersWhoHaveEarnedSilverCompetitorBadge = playersWhoHaveEarnedSilverCompetitorBadge, PlayersWhoHaveEarnedBronzeWarriorBadge = playersWhoHaveEarnedBronzeWarriorBadge});
        }
	}
}