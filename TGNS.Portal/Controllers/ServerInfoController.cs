using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Core.Domain;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class ServerInfoController : Controller
    {
        private readonly IServerCurrentInfoDictionaryGetter _serverCurrentInfoDictionaryGetter;
        private readonly IServerGetter _serverGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;

        public ServerInfoController()
        {
            _serverGetter = new ServerGetter();
            _serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
        }

        IServerCurrentInfo GetServerCurrentInfo(IServerModel serverModel)
        {
            var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
            var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
            var serverPlayers = serverCurrentInfo.Players.ToList();
            var playersToShow = serverPlayers.Where(x => !x.IsBot).ToList();
            var firstBot = serverPlayers.FirstOrDefault(x => x.IsBot);
            if (firstBot != default(IServerPlayer))
            {
                playersToShow.Add(firstBot);
            }
            var result = new ServerCurrentInfo(serverCurrentInfo.MapName, playersToShow, serverCurrentInfo.ServerName);
            return result;
        }

        IDictionary<string, object> GetJsonDictionary(IServerCurrentInfo serverCurrentInfo)
        {
            var players = serverCurrentInfo.Players.OrderBy(x => x.Team.Name).ThenBy(x => x.PlayerName).Select(currentPlayer => new Dictionary<string, object>
            {
                {"name", currentPlayer.PlayerName}, {"id", currentPlayer.PlayerId},
                {
                    "team", new Dictionary<string, object>
                    {
                        {"name", currentPlayer.Team.Name}, {"number", currentPlayer.Team.Number}
                    }
                },
                {"score", currentPlayer.Score},
                {"resources", currentPlayer.Resources},
                {"playerId", currentPlayer.PlayerId},
                {"iscomm", currentPlayer.IsCommander},
                {"isbot", currentPlayer.IsBot}
            });
            var result = new Dictionary<string, object>
            {
                {"serverName", serverCurrentInfo.ServerName},
                {"mapName", serverCurrentInfo.MapName},
                {"players", players}
            };
            return result;
        }
            
            
        [HttpGet]
        public ActionResult Index()
        {
            var serverModels = _serverGetter.Get();
            var serverCurrentInfos = serverModels.Select(GetServerCurrentInfo);
            var serverInfoViewModel = new ServerInfoViewModel(serverCurrentInfos);
            return View(serverInfoViewModel);
        }

        [HttpGet]
        public JsonResult v1_0()
        {
            var serverModels = _serverGetter.Get();
            var serverCurrentInfos = serverModels.Select(GetServerCurrentInfo);
            var result = serverCurrentInfos.Select(GetJsonDictionary);
            var resultJson = Json(result, JsonRequestBehavior.AllowGet);
            return resultJson;
        }
    }
}