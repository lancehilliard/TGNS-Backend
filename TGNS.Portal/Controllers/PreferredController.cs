using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class PreferredController : AdminController
    {
        private readonly IPreferredEntriesGetter _preferredEntriesGetter;
        private readonly IPreferredEntryRemover _preferredEntryRemover;
        private readonly IPreferredEntriesSetter _preferredEntriesSetter;
        private readonly IPreferredEntryAdder _preferredEntryAdder;

        public PreferredController()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Data"].ConnectionString;
            _preferredEntriesGetter = new PreferredEntriesGetter(connectionString);
            _preferredEntriesSetter = new PreferredEntriesSetter(connectionString);
            _preferredEntryAdder = new PreferredEntryAdder(connectionString, _preferredEntriesGetter, _preferredEntriesSetter);
            _preferredEntryRemover = new PreferredEntryRemover(connectionString, _preferredEntriesGetter, _preferredEntriesSetter);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var preferredEntries = _preferredEntriesGetter.Get("ns2");
            var preferredViewModel = new PreferredViewModel(preferredEntries);
            return View(preferredViewModel);
        }

        [HttpPost]
        public ActionResult Add(long playerId, string pluginName)
        {
            var preferredEntryToRemove = new PreferredEntry(playerId, pluginName);
            _preferredEntryAdder.Add("ns2", preferredEntryToRemove);
            TempData["Success"] = string.Format("{0} preferred for {1}.", playerId, pluginName);
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public ActionResult Delete(long playerId, string pluginName)
        {
            var preferredEntries = _preferredEntriesGetter.Get("ns2");
            var matchingPreferredEntries = preferredEntries.Where(x => Equals(playerId, x.PlayerId) && Equals(pluginName, x.PluginName));
            foreach (var matchingPreferredEntry in matchingPreferredEntries)
            {
                _preferredEntryRemover.Remove("ns2", matchingPreferredEntry);
            }
            TempData["Success"] = string.Format("Preferred removed for {0} from {1}.", playerId, pluginName);
            return RedirectToAction("Manage");
        }
	}
}