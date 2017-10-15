using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class BlacklistController : Controller
    {
        private readonly IBlacklistEntriesGetter _blacklistEntriesGetter;
        private readonly IBlacklistEntryRemover _blacklistEntryRemover;
        private readonly IBlacklistEntriesSetter _blacklistEntriesSetter;
        private readonly IBlacklistEntryAdder _blacklistEntryAdder;

        public BlacklistController()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Data"].ConnectionString;
            _blacklistEntriesGetter = new BlacklistEntriesGetter(connectionString);
            _blacklistEntriesSetter = new BlacklistEntriesSetter(connectionString);
            _blacklistEntryAdder = new BlacklistEntryAdder(connectionString, _blacklistEntriesGetter, _blacklistEntriesSetter);
            _blacklistEntryRemover = new BlacklistEntryRemover(connectionString, _blacklistEntriesGetter, _blacklistEntriesSetter);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var blacklistEntries = _blacklistEntriesGetter.Get("ns2");
            var blacklistViewModel = new BlacklistViewModel(blacklistEntries);
            return View(blacklistViewModel);
        }

        [HttpPost]
        public ActionResult Add(long playerId, string from)
        {
            var blacklistEntryToRemove = new BlacklistEntry(playerId, from);
            _blacklistEntryAdder.Add("ns2", blacklistEntryToRemove);
            TempData["Success"] = string.Format("{0} blacklisted from {1}.", playerId, from);
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public ActionResult Delete(long playerId, string @from)
        {
            var blacklistEntries = _blacklistEntriesGetter.Get("ns2");
            var matchingBlacklistEntries = blacklistEntries.Where(x=>Equals(playerId, x.PlayerId) && Equals(from, x.From));
            foreach (var matchingBlacklistEntry in matchingBlacklistEntries)
            {
                _blacklistEntryRemover.Remove("ns2", matchingBlacklistEntry);
            }
            TempData["Success"] = string.Format("Blacklist removed for {0} from {1}.", playerId, from);
            return RedirectToAction("Manage");
        }
    }
}