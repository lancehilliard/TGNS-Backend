using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class BadgesController : AuthenticatedController
    {
        private readonly BadgePlayerGetter _badgePlayerGetter;
        private readonly BadgePlayerAdapter _badgePlayerAdapter;

        public BadgesController()
        {
            _badgePlayerGetter = new BadgePlayerGetter();
            _badgePlayerAdapter = new BadgePlayerAdapter();
        }

        //
        // GET: /Badges/
        public ActionResult Index()
        {
            // todo mlh delete this action?
            return View();
        }

        [HttpPost]
        public ActionResult Manage(string selectedBadgeInfo)
        {
            var parts = selectedBadgeInfo.Split('|');
            var selectedBadgeId = Convert.ToInt32(parts[0]);
            var selectedBadgeDisplayName = parts[1];
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE achievements_badges_players SET ShowInGame = 0 WHERE achievementsrealm = 'ns2' and playerid = @PlayerId;UPDATE achievements_badges_players SET ShowInGame = 1 WHERE achievementsrealm = 'ns2' and playerid = @PlayerId and badgeid = @BadgeId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", PlayerId);
                    command.Parameters.AddWithValue("@BadgeId", selectedBadgeId);
                    command.ExecuteNonQuery();
                }
            }
            TempData["Success"] = string.Format("{0} selected.", selectedBadgeDisplayName);
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var badgePlayers = _badgePlayerGetter.Get(PlayerId);
            var viewModel = badgePlayers.Select(_badgePlayerAdapter.Adapt);
            return View(viewModel);
        }
	}
}