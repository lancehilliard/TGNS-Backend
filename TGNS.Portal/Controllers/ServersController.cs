using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using TGNS.Core.Commands;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class ServersController : AuthenticatedController
    {
        readonly IServerGetter _serverGetter;
        readonly IServerAdapter _serverAdapter;
        readonly IServerMapCycler _serverMapCycler;
        private readonly ServerCurrentInfoDictionaryDictionaryGetter _serverCurrentInfoDictionaryDictionaryGetter;
        private readonly GuardianOptinStatusReader _guardianOptinStatusReader;
        private readonly BalanceTotalGamesCountGetter _balanceTotalGamesCountGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;
        private readonly IServerProcessCommandSender _serverProcessCommandSender;

        public ServersController()
        {
            _serverGetter = new ServerGetter();
            _serverAdapter = new ServerAdapter();
            _serverMapCycler = new ServerMapCycler();
            _serverCurrentInfoDictionaryDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _guardianOptinStatusReader = new GuardianOptinStatusReader();
            _balanceTotalGamesCountGetter = new BalanceTotalGamesCountGetter();
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
            _serverProcessCommandSender = new ServerProcessCommandSender();
        }

        bool PlayerQualifies
        {
            get
            {
                bool? result = ViewBag.PlayerQualifies;
                if (!result.HasValue)
                {
                    var balanceTotalGames = _balanceTotalGamesCountGetter.Get(PlayerId) ?? 0;
                    var isOptedIntoGuardian = _guardianOptinStatusReader.IsOptedIn(PlayerId);
                    result = isOptedIntoGuardian && balanceTotalGames >= 40;
                    ViewBag.PlayerQualifies = result;
                }
                return result.Value;
            }
        }

        [HttpPost]
        public ActionResult Restart(int serverId)
        {
            var server = _serverGetter.Get().Single(x => x.ID.Equals(serverId));
            if (PlayerQualifies)
            {
                var cacheKey = string.Format("{0}RestartedWhen{1}", UserName, server.ID);
                var whenLastRestarted = Convert.ToDateTime(HttpContext.Cache[cacheKey] ?? DateTime.MinValue);
                if (whenLastRestarted <= DateTime.Now.AddMinutes(-2))
                {
                    var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(server);
                    if (!(bool)serverCurrentInfoDictionary["success"])
                    {
                        Thread.Sleep(5000);
                        serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(server);
                        if (!(bool) serverCurrentInfoDictionary["success"])
                        {
                            Thread.Sleep(5000);
                            serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(server);
                            if (!(bool)serverCurrentInfoDictionary["success"])
                            {
                                _serverProcessCommandSender.Restart(serverId);
                                TempData["Success"] = string.Format("Restart performed on {0}.", server.Name);
                                HttpContext.Cache[cacheKey] = DateTime.Now;
                            }
                            else
                            {
                                TempData["Error"] = "Server queried successfully. Restart halted.";
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Server queried successfully. Restart halted.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Server queried successfully. Restart halted.";
                    }
                }
                else
                {
                    TempData["Error"] = string.Format("Wait before again restarting {0}.", server.Name);
                }
            }
            else
            {
                TempData["Error"] = "You are not qualified to use this command. Restart halted.";
            }
            return RedirectToAction("Restart");
        }

        [HttpPost]
        public ActionResult Reset(int serverId)
        {
            var server = _serverGetter.Get().Single(x => x.ID.Equals(serverId));
            if (PlayerQualifies)
            {
                var cacheKey = string.Format("{0}ResetWhen{1}", UserName, server.ID);
                var whenLastReset = Convert.ToDateTime(HttpContext.Cache[cacheKey] ?? DateTime.MinValue);
                if (whenLastReset <= DateTime.Now.AddMinutes(-2))
                {
                    var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(server);
                    var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                    var currentNonReadyRoomPlayerCount = serverCurrentInfo.Players.Count(x => !x.IsReadyRoom);
                    if (currentNonReadyRoomPlayerCount == 0)
                    {
                        if (_serverMapCycler.Cycle(UserName, PlayerId, server.WebAdminBaseUrl))
                        {
                            TempData["Success"] = string.Format("Reset performed on {0}.", server.Name);
                            HttpContext.Cache[cacheKey] = DateTime.Now;
                        }
                        else
                        {
                            TempData["Error"] = string.Format("Error resetting {0}.", server.Name);
                        }

                    }
                    else
                    {
                        TempData["Error"] = string.Format("{0} has non-ReadyRoom players. Reset halted.", server.Name);
                    }
                }
                else
                {
                    TempData["Error"] = string.Format("Wait before again resetting {0}.", server.Name);
                }
            }
            else
            {
                TempData["Error"] = "You are not qualified to use this command. Reset halted."; 
            }
            return RedirectToAction("Reset");
        }

        [HttpGet]
        public ActionResult Reset()
        {
            var serverViewModels = new List<ServerViewModel>();
            ViewBag.PlayerHasGuardian = PlayerQualifies;
            if (PlayerQualifies)
            {
                var serverModels = _serverGetter.Get();

                foreach (var serverModel in serverModels)
                {
                    var cacheKey = string.Format("{0}ResetWhen{1}", UserName, serverModel.ID);
                    var whenLastReset = Convert.ToDateTime(HttpContext.Cache[cacheKey] ?? DateTime.MinValue);
                    if (whenLastReset <= DateTime.Now.AddMinutes(-2))
                    {
                        var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(serverModel);
                        var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                        var currentNonReadyRoomPlayerCount = serverCurrentInfo.Players.Count(x => !x.IsReadyRoom);
                        if (currentNonReadyRoomPlayerCount == 0)
                        {
                            serverViewModels.Add(_serverAdapter.Adapt(serverModel));
                        }
                    }
                }
            }
            return View(serverViewModels);
        }

        [HttpGet]
        public ActionResult Restart()
        {
            var serverViewModels = new List<ServerViewModel>();
            ViewBag.PlayerHasGuardian = PlayerQualifies;
            if (PlayerQualifies)
            {
                var serverModels = _serverGetter.Get();

                foreach (var serverModel in serverModels)
                {
                    var cacheKey = string.Format("{0}RestartedWhen{1}", UserName, serverModel.ID);
                    var whenLastRestarted = Convert.ToDateTime(HttpContext.Cache[cacheKey] ?? DateTime.MinValue);
                    if (whenLastRestarted <= DateTime.Now.AddMinutes(-2))
                    {
                        var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(serverModel);
                        if (!(bool)serverCurrentInfoDictionary["success"])
                        {
                            serverViewModels.Add(_serverAdapter.Adapt(serverModel));
                        }
                    }
                }
            }
            return View(serverViewModels);
        }
    }
}