using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class GamesController : AdminController
    {
        private readonly IPlayedGamesGetter _playedGamesGetter;
        public GamesController()
        {
            _playedGamesGetter = new PlayedGamesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var playedGames = _playedGamesGetter.Get("ns2", PlayerId).Where(x=>x.Created > DateTime.Now.AddDays(-14)).OrderByDescending(x=>x.Created);
            return View(playedGames);
        }
	}
}