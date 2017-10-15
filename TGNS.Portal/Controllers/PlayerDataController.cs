using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Core.Steam;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class PlayerDataController : Controller
    {
        readonly IPlayerAliasesGetter _playerAliasesGetter;
        private readonly ISteamIdConverter _steamIdConverter;

        public PlayerDataController()
        {
            _playerAliasesGetter = new PlayerAliasesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _steamIdConverter = new SteamIdConverter();
        }

        [HttpGet]
        public JsonResult GetAliases(long playerId)
        {
            if (playerId > 76560000000000000)
            {
                playerId = _steamIdConverter.GetNs2IdFrom64BitSteamId(playerId.ToString());
            }
            var resultData = _playerAliasesGetter.Get(playerId);
            var result = Json(new {playerId=playerId, aliases= resultData }, JsonRequestBehavior.AllowGet);
            return result;
        }

        [HttpGet]
        public RedirectResult RedirectToSteamCommunityProfile(long id)
        {
            var url = string.Format("http://steamcommunity.com/profiles/{0}", _steamIdConverter.GetSteamCommunityProfileIdFromNs2Id(id));
            return Redirect(url);
        }

        [HttpGet]
        public JsonResult GetIdentities(string partialPlayerName)
        {
            var resultData = _playerAliasesGetter.Get(partialPlayerName);
            var result = Json(resultData, JsonRequestBehavior.AllowGet);
            return result;
        }

    }
}