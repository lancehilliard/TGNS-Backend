using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class KarmaController : AuthenticatedController
    {
        private readonly IPlayerAdminChecker _playerAdminChecker;
        private readonly IKarmaGetter _karmaGetter;
        private readonly IBkaDataGetter _bkaDataGetter;

        public KarmaController()
        {
            _playerAdminChecker = new PlayerAdminChecker();
            _karmaGetter = new KarmaGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _bkaDataGetter = new BkaDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
        }

        public ActionResult Index(long? id)
        {
            if (id.HasValue)
            {
                if (id != PlayerId && !_playerAdminChecker.IsAdmin(PlayerId))
                {
                    throw new Exception("Disallowed request.");
                }
            }
            else
            {
                id = PlayerId;
            }
            var bkaData = _bkaDataGetter.Get(id.Value);
            var bka = bkaData != null ? bkaData.Bka : string.Empty;
            var karmaDeltas = _karmaGetter.Get("ns2", id.Value);
            return View(new KarmaViewModel { PlayerId = id.Value, Bka = bka, Deltas = karmaDeltas });
        }
    }
}