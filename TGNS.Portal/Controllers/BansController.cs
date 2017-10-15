using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class BansController : AdminController
    {
        private readonly IBansGetter _bansGetter;
        private readonly IBanAdapter _banAdapter;
        private readonly IBanRemover _banRemover;
        private readonly IServerGetter _serverGetter;
        private readonly IServerAdminCommandSender _serverAdminCommandSender;
        private readonly IBanAdder _banAdder;

        public BansController()
        {
            _bansGetter = new BansGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _banRemover = new BanRemover(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _banAdapter = new BanAdapter();
            _serverGetter = new ServerGetter();
            _serverAdminCommandSender = new ServerAdminCommandSender();
            _banAdder = new BanAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var bans = _bansGetter.Get("ns2").OrderByDescending(x=>x.LastModified);
            var viewModel = bans.Select(_banAdapter.Adapt);
            return View(viewModel);
        }
    
        [HttpGet]
        public ActionResult Delete(long id)
        {
            var bans = _bansGetter.Get("ns2");
            var firstBanForPlayer = bans.FirstOrDefault(x => Equals(id, x.PlayerId));
            if (firstBanForPlayer != null)
            {
                _banRemover.Remove("ns2", firstBanForPlayer.PlayerId);
                ForceBanSync();
                TempData["Success"] = string.Format("Ban removed for {0} ({1}).", firstBanForPlayer.PlayerName, firstBanForPlayer.PlayerId);
            }
            else
            {
                TempData["Error"] = string.Format("No ban found for {0}.", id);
            }
            return RedirectToAction("Manage");
        }

        private void ForceBanSync()
        {
            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                _serverAdminCommandSender.Send(serverModel.WebAdminBaseUrl, UserName, PlayerId, "sh_forcebansync", false);
            }
        }

        [HttpPost]
        public RedirectToRouteResult Add(long targetPlayerId, string reason)
        {
            try
            {
                _banAdder.Add(targetPlayerId, reason, PlayerId, UserName);
                var serverModels = _serverGetter.Get();
                foreach (var serverModel in serverModels)
                {
                    _serverAdminCommandSender.Send(serverModel.WebAdminBaseUrl, UserName, PlayerId, string.Format("sh_banid {0} 0 {1}", targetPlayerId, reason), false);
                }
                ForceBanSync();
                TempData["Success"] = string.Format("Ban added for {0}.", targetPlayerId);
            }
            catch (Exception e)
            {
                TempData["Error"] = string.Format("Error adding ban. Is {0} already banned?", targetPlayerId);
                TempData["TargetPlayerId"] = targetPlayerId;
                TempData["Reason"] = reason;
            }
            return RedirectToAction("Manage");
        }
    }
}