using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class ReplayController : Controller
    {
        private readonly IBkaDataGetter _bkaDataGetter;
        private readonly IGameRecordingsGetter _gameRecordingsGetter;
        private readonly IGamesGetter _gamesGetter;
        private readonly IPlayedGamesGetter _playedGamesGetter;
        private readonly int POSTGAME_PREREADYROOM_EXPECTED_DURATION_IN_SECONDS = 7;
        private readonly int PREGAME_COUNTDOWN_EXPECTED_DURATION_IN_SECONDS = 6;
        private readonly IGameRecordingProviderStatisticsGetter _gameRecordingProviderStatisticsGetter;
        private readonly IGameRecordingDurationGetter _gameRecordingDurationGetter;
        private readonly IReplayGamesDatatablesDataGetter _replayGamesDatatablesDataGetter;

        public ReplayController()
        {
            _bkaDataGetter = new BkaDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
            _gameRecordingsGetter = new GameRecordingsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gamesGetter = new GamesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _playedGamesGetter = new PlayedGamesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gameRecordingProviderStatisticsGetter = new GameRecordingProviderStatisticsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _replayGamesDatatablesDataGetter = new ReplayGamesDatatablesDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gameRecordingDurationGetter = new GameRecordingDurationGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        public ActionResult Index(string rating, string count, string duration)
        {
            var videoStatistics = _gameRecordingProviderStatisticsGetter.Get();
            var totalGamesDurationInSeconds = _gameRecordingDurationGetter.GetTotalGamesDurationInSeconds();
            var distinctPlayerCount = _gameRecordingsGetter.GetDistinctPlayerCount();

            //var recordedGames = _gameRecordingsGetter.GetRecordedGames().OrderByDescending(x=>x.StartTimeSeconds);
            //var games = recordedGames.Select(x => _gamesGetter.Get(x.ServerName, x.StartTimeSeconds));
            //var ns2Games = games.Where(x => x.GameMode == "ns2");
            //var infestedGames = games.Where(x => x.GameMode == "Infested");

            //var ratingFilterOptions = new List<SelectListItem> { new SelectListItem { Text = "any", Value = "" }, new SelectListItem { Text = "3+", Value = "3" }, new SelectListItem { Text = "4+", Value = "4" } };
            //if (!string.IsNullOrWhiteSpace(rating))
            //{
            //    ratingFilterOptions = ratingFilterOptions.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = x.Value == rating }).ToList();
            //    ns2Games = ns2Games.Where(x => x.RatingsAverage >= Convert.ToInt32(rating));
            //    infestedGames = infestedGames.Where(x => x.RatingsAverage >= Convert.ToInt32(rating));
            //}

            //var durationFilterOptions = new List<SelectListItem> { new SelectListItem { Text = "any", Value = "" }, new SelectListItem { Text = "10+", Value = "10" }, new SelectListItem { Text = "15+", Value = "15" }, new SelectListItem { Text = "30+", Value = "30" }, new SelectListItem { Text = "45+", Value = "45" }, new SelectListItem { Text = "60+", Value = "60" } };
            //if (!string.IsNullOrWhiteSpace(duration))
            //{
            //    durationFilterOptions = durationFilterOptions.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = x.Value == duration }).ToList();
            //    ns2Games = ns2Games.Where(x => x.DurationInSeconds >= Convert.ToInt32(duration) * 60);
            //    infestedGames = infestedGames.Where(x => x.DurationInSeconds >= Convert.ToInt32(duration) * 60);
            //}

            //var countFilterOptions = new List<SelectListItem> { new SelectListItem { Text = "all", Value = "" }, new SelectListItem { Text = "25", Value = "25" }, new SelectListItem { Text = "50", Value = "50" }, new SelectListItem { Text = "250", Value = "250" }, new SelectListItem { Text = "500", Value = "500" } };
            //if (count == null)
            //{
            //    count = countFilterOptions.First(x => !string.IsNullOrWhiteSpace(x.Value)).Value;
            //}
            //if (!string.IsNullOrWhiteSpace(count))
            //{
            //    countFilterOptions = countFilterOptions.Select(x => new SelectListItem { Text = x.Text, Value = x.Value, Selected = x.Value == count }).ToList();
            //    ns2Games = ns2Games.Take(Convert.ToInt32(count));
            //    infestedGames = infestedGames.Take(Convert.ToInt32(count));
            //}

            //return View(new RecordingIndexViewModel { Ns2Games = ns2Games.Where(x => x.DurationInSeconds >= 60), InfestedGames = infestedGames.Where(x => x.DurationInSeconds >= 60), RatingFilterOptions = ratingFilterOptions, CountFilterOptions = countFilterOptions, DurationFilterOptions = durationFilterOptions, VideoStatistics = videoStatistics });
            return View(new RecordingIndexViewModel { VideoStatistics = videoStatistics, TotalGamesDurationInSeconds = totalGamesDurationInSeconds, DistinctPlayerCount = distinctPlayerCount });
        }

        public ActionResult Watch(string serverName, double startTimeSeconds)
        {
            var cameraOptions = new List<SelectListItem>();
            var gameRecordings = _gameRecordingsGetter.Get(serverName, startTimeSeconds);
            foreach (var gameRecording in gameRecordings)
            {
                var bkaData = _bkaDataGetter.Get(gameRecording.PlayerId);
                if (!string.IsNullOrWhiteSpace(bkaData?.Bka))
                {
                    var playedGame = _playedGamesGetter.Get("ns2", gameRecording.PlayerId).SingleOrDefault(x => x.ServerName.Equals(serverName) && x.StartTimeSeconds.Equals(startTimeSeconds));
                    string cameraTeamInfo = null;
                    if (playedGame != null)
                    {
                        if (playedGame.GameMode == "ns2")
                        {
                            var team = playedGame.MarineSeconds > playedGame.AlienSeconds ? "M" : "A";
                            var comm = playedGame.CommanderSeconds > playedGame.DurationInSeconds/2 ? "C" : string.Empty;
                            cameraTeamInfo = $"{team}{comm}";
                        }
                    }
                    else
                    {
                        cameraTeamInfo = "S";
                    }
                    var cameraDisplay = $"{bkaData.Bka}";
                    if (!string.IsNullOrWhiteSpace(cameraTeamInfo))
                    {
                        cameraDisplay = $"{cameraDisplay} ({cameraTeamInfo})";
                    }
                    cameraOptions.Add(new SelectListItem { Value = gameRecording.VideoIdentifier, Text = cameraDisplay });
                }
            }
            return View(new RecordingWatchViewModel { CameraOptions = cameraOptions });
        }

        public ActionResult RecordingHelper()
        {
            return View();
        }

        public ActionResult AddHelperChromeExtension()
        {
            return View();
        }

        public ActionResult AddHelperUserscript()
        {
            return View();
        }

        public ActionResult ContributorsGuide()
        {
            return View();
        }

        public JsonResult ReplayData(string gameMode)
        {
            var globalRatingsAverage = _replayGamesDatatablesDataGetter.GetGlobalRatingsAverage(gameMode);
            var replayGames = _replayGamesDatatablesDataGetter.Get(gameMode, globalRatingsAverage);
            var resultJson = Json(new Dictionary<string, object> { {"data", replayGames} }, JsonRequestBehavior.AllowGet);
            resultJson.MaxJsonLength = int.MaxValue;
            return resultJson;
        }
    }
}