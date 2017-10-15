using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class FullSpecController : AuthenticatedController
    {
        private readonly FullSpecOptionChanger _fullSpecOptionChanger;
        private readonly ServerAdminCommandSender _serverAdminCommandSender;
        private readonly FullSpecOptionsOptionsModelGetter _fullSpecOptionsModelGetter;
        private readonly ServerCurrentInfoDictionaryDictionaryGetter _serverCurrentInfoDictionaryDictionaryGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;

        public FullSpecController()
        {
            _fullSpecOptionsModelGetter = new FullSpecOptionsOptionsModelGetter();
            _fullSpecOptionChanger = new FullSpecOptionChanger();
            _serverAdminCommandSender = new ServerAdminCommandSender();
            _serverCurrentInfoDictionaryDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var serverModels = _serverGetter.Get();
            var serverSpectatorCounts = new Dictionary<string, int>();

            foreach (var serverModel in serverModels)
            {
                var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryDictionaryGetter.Get(serverModel);
                var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                var currentSpectatorCount = serverCurrentInfo.Players.Count(x => x.IsSpectator);
                serverSpectatorCounts.Add(serverModel.Name, currentSpectatorCount);
            }
            var fullSpecOptionsModel = _fullSpecOptionsModelGetter.Get();
            var optedIn = fullSpecOptionsModel.EnrolledPlayerIds.Any(x => x == PlayerId);
            var currentStatus = optedIn ? "opted in" : "opted out";
            ViewBag.CurrentStatus = string.Format("You are currently {0}.", currentStatus);
            ViewBag.OptedIn = optedIn;
            ViewBag.ServerSpectatorCounts = serverSpectatorCounts;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(bool optIn)
        {
            string message;
            if (optIn)
            {
                _fullSpecOptionChanger.OptIn(PlayerId);
                message = "You are now opted into sh_fullspec.";
            }
            else
            {
                _fullSpecOptionChanger.OptOut(PlayerId);
                message = "You are now opted out of sh_fullspec.";
            }
            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                _serverAdminCommandSender.Send(serverModel.WebAdminBaseUrl, UserName, PlayerId, "sh_fullspec_datarefresh", false);
            }
            TempData["Success"] = message;
            return RedirectToAction("Manage", "FullSpec");
        }
    }
}