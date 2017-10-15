using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class BucketsController : AdminController
    {
        private readonly IBucketsGetter _bucketsGetter;
        private readonly IBucketAdapter _bucketAdapter;
        private readonly IServerCurrentInfoDictionaryGetter _serverCurrentInfoDictionaryGetter;
        private readonly IServerGetter _serverGetter;
        private readonly IBucketsPlayerAdder _bucketsPlayerAdder;
        private readonly IDictionary<string, Action<IBucketPlayer>> _bucketNamePlayerAddActions;
        private readonly IDictionary<string, Action<long>> _bucketNamePlayerRemoveActions;
        private readonly IBucketsPlayerRemover _bucketsPlayerRemover;
        private readonly IRecentPlayersGetter _recentPlayersGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;

        public BucketsController()
        {
            _bucketsGetter = new BucketsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _bucketAdapter = new BucketAdapter();
            _serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _serverGetter = new ServerGetter();
            _bucketsPlayerAdder = new BucketsPlayerAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _bucketsPlayerRemover = new BucketsPlayerRemover(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _bucketNamePlayerAddActions = new Dictionary<string, Action<IBucketPlayer>>
            {
                {"Commanders", player => _bucketsPlayerAdder.AddCommander(player)},
                {"BestPlayers", player => _bucketsPlayerAdder.AddBestPlayer(player)},
                {"BetterPlayers", player => _bucketsPlayerAdder.AddBetterPlayer(player)},
                {"GoodPlayers", player => _bucketsPlayerAdder.AddGoodPlayer(player)}
            };
            _bucketNamePlayerRemoveActions = new Dictionary<string, Action<long>>
            {
                {"Commanders", playerId => _bucketsPlayerRemover.RemoveCommander(playerId)},
                {"BestPlayers", playerId => _bucketsPlayerRemover.RemoveBestPlayer(playerId)},
                {"BetterPlayers", playerId => _bucketsPlayerRemover.RemoveBetterPlayer(playerId)},
                {"GoodPlayers", playerId => _bucketsPlayerRemover.RemoveGoodPlayer(playerId)}
            };
            _recentPlayersGetter = new RecentPlayersGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
        }

        private IList<string> Logs
        {
            get
            {
                var result = HttpContext.Cache["BucketLogs"] as IList<string>;
                if (result == null)
                {
                    result = new List<string>();
                    HttpContext.Cache.Insert("BucketLogs", result, null, DateTime.MaxValue, Cache.NoSlidingExpiration);
                }
                return result;
            }
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var buckets = _bucketsGetter.Get();

            var recentPlayers = HttpContext.Cache["RecentPlayers"] as IEnumerable<IBucketPlayer>;
            if (recentPlayers == null)
            {
                recentPlayers = _recentPlayersGetter.Get("ns2");
                HttpContext.Cache.Insert("RecentPlayers", recentPlayers, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0));
            }
            var viewModel = _bucketAdapter.Adapt(buckets);
            viewModel.RecentPlayers = recentPlayers.Where(x => !(buckets.CommPlayers.Any(p => Equals(p.Id, x.Id)) || buckets.BestPlayers.Any(p => Equals(p.Id, x.Id)) || buckets.BetterPlayers.Any(p => Equals(p.Id, x.Id)) || buckets.GoodPlayers.Any(p => Equals(p.Id, x.Id)))).Take(100);
            viewModel.Logs = Logs.Take(50);
            return View(viewModel);
        }

        [HttpGet]
        public JsonResult SearchPlayers(string q)
        {
            var players = new List<IServerPlayer>();
            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
                var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                players.AddRange(serverCurrentInfo.Players);
            }
            var recentPlayers = _recentPlayersGetter.Get("ns2").ToList();
            foreach (var player in players)
            {
                if (!recentPlayers.Any(x => Equals(x.Id, player.PlayerId)))
                {
                    recentPlayers.Add(new BucketPlayer(player.PlayerName, player.PlayerId));
                }
            }
            var resultData = recentPlayers.Where(x => x.Name.ToLower().Contains(q.ToLower())).Select(x => new { name = x.Name, id = x.Id }).ToList();
            var result = Json(resultData, JsonRequestBehavior.AllowGet);
            return result;
        }

        [HttpPost]
        public ActionResult Add(long playerId, string playerName, string bucketName)
        {
            var player = new BucketPlayer(playerName, playerId);
            _bucketNamePlayerAddActions[bucketName](player);
            var activity = string.Format("Added {0} ({1}) to {2} bucket.", playerName, playerId, bucketName);
            TempData["Success"] = activity;
            Logs.Insert(0, string.Format("{0} {1} {2}: {3}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), UserName, activity));
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public ActionResult Remove(long playerId, string playerName, string bucketName)
        {
            _bucketNamePlayerRemoveActions[bucketName](playerId);
            var activity = string.Format("Removed {0} ({1}) from {2} bucket.", playerName, playerId, bucketName);
            TempData["Success"] = activity;
            Logs.Insert(0, string.Format("{0} {1} {2}: {3}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), UserName, activity));
            return RedirectToAction("Manage");
        }
    }
}