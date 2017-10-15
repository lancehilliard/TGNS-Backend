using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using System.Xml;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using SteamKit2;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Core.Steam;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;
using WebGrease.Css.Extensions;

// http://stackoverflow.com/questions/19896430/keeping-two-youtube-videos-in-sync-with-each-other

// http://stackoverflow.com/a/3652479/116895
namespace TGNS.Portal.Controllers
{
    public class RecordingsController : AuthenticatedController
    {
        private readonly IBkaDataGetter _bkaDataGetter;
        private readonly IGameRecordingsGetter _gameRecordingsGetter;
        private readonly IGameRecordingAdder _gameRecordingAdder;
        private readonly IPlayedGamesGetter _playedGamesGetter;
        private readonly int POSTGAME_PREREADYROOM_EXPECTED_DURATION_IN_SECONDS = 7;
        private readonly int PREGAME_COUNTDOWN_EXPECTED_DURATION_IN_SECONDS = 6;
        private readonly IGamesGetter _gamesGetter;
        private readonly IGameRecordingRemover _gameRecordingRemover;
        private readonly IPlayerAdminChecker _playerAdminChecker;
        private readonly IKarmaAdder _karmaAdder;
        private readonly IGameRecordingKarmaChecker _gameRecordingKarmaChecker;
        private readonly IGameRecordingKarmaAdder _gameRecordingKarmaAdder;
        private readonly IPlayerIdAdapter _playerIdAdapter;
        private readonly IServerAdminCommandSender _serverAdminCommandSender;
        private readonly ISteamIdConverter _steamIdConverter;
        private readonly IList<long> ReplayNotificationOptOutNs2Ids = new List<long> {104330739};

        public RecordingsController()
        {
            _playedGamesGetter = new PlayedGamesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _bkaDataGetter = new BkaDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
            _gameRecordingAdder = new GameRecordingAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gamesGetter = new GamesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gameRecordingsGetter = new GameRecordingsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gameRecordingRemover = new GameRecordingRemover(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _playerAdminChecker = new PlayerAdminChecker();
            _karmaAdder = new KarmaAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gameRecordingKarmaChecker = new GameRecordingKarmaChecker(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _gameRecordingKarmaAdder = new GameRecordingKarma(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _playerIdAdapter = new PlayerIdAdapter();
            _serverAdminCommandSender = new ServerAdminCommandSender();
        }

        public ActionResult Delete()
        {

            var playerIsAdmin = _playerAdminChecker.IsAdmin(PlayerId);
            var recordedGames = _gameRecordingsGetter.GetRecordedGames().OrderByDescending(x => x.StartTimeSeconds);
            var deletableGameRecordings = new List<IDeletableGameRecording>();
            var bkaDatas = _bkaDataGetter.Get().ToList();
            foreach (var recordedGame in recordedGames)
            {
                var gameRecordings = _gameRecordingsGetter.Get(recordedGame.ServerName, recordedGame.StartTimeSeconds);
                foreach (var gameRecording in gameRecordings)
                {
                    var userMayDelete = playerIsAdmin || gameRecording.PlayerId.Equals(PlayerId);
                    if (userMayDelete)
                    {
                        var bkaData = bkaDatas.SingleOrDefault(x => x.PlayerId.Equals(gameRecording.PlayerId));
                        var game = _gamesGetter.Get(recordedGame.ServerName, recordedGame.StartTimeSeconds);
                        deletableGameRecordings.Add(new DeletableGameRecording(game, gameRecording.PlayerId, bkaData));
                    }
                }
            }
            return View(new RecordingsDeleteViewModel {DeletableGameRecordings = deletableGameRecordings});
        }

        [HttpGet]
        public ActionResult Add()
        {
            var recentGamesOptions = GetRecentGamesOptions(GetRecentGames());
            return View(new RecordingsAddViewModel {GamesOptions = recentGamesOptions});
        }

        private IEnumerable<SelectListItem> GetRecentGamesOptions(IOrderedEnumerable<IGame> recentGames)
        {
            var result = recentGames.Select(x => new SelectListItem {Value = x.StartTimeSeconds.ToString(CultureInfo.InvariantCulture), Text = $"{GetFormattedStartTimeSeconds(x.StartTimeSeconds)} - {x.MapName} {x.Created.ToShortDateString()} [{TimeSpan.FromSeconds(x.DurationInSeconds).ToString(@"mm\:ss")}]"}).ToList();
            result.Insert(0, new SelectListItem {Text="Select Game", Value="0"});
            return result;
        }

        private string GetFormattedStartTimeSeconds(double startTimeSeconds)
        {
            var s = startTimeSeconds.ToString(CultureInfo.InvariantCulture);
            var length = s.Length;
            string result = $"{s.Substring(0, length-4)} {s.Substring(length-4)}";
            return result;
        }

        private IOrderedEnumerable<IGame> GetRecentGames()
        {
            return _gamesGetter.Get(DateTime.Now.AddDays(-30)).Where(x => !x.IncludedBots && x.Realm.Equals("ns2") && x.DurationInSeconds > 0).OrderByDescending(x => x.Created);
        }

        [HttpPost]
        public ActionResult Add(double gameStartTimeSeconds, string videoIdentifier, bool returnJson)
        {
            var recentGames = GetRecentGames();
            var recentGamesOptions = GetRecentGamesOptions(recentGames).ToList();
            var selectedGameValue = gameStartTimeSeconds.ToString(CultureInfo.InvariantCulture);
            var attemptResult = AttemptAdd(gameStartTimeSeconds, videoIdentifier, recentGames, recentGamesOptions);
            if (attemptResult.Success)
            {
                TempData["Success"] = $"Added recording {videoIdentifier} for game {selectedGameValue}!";
                videoIdentifier = string.Empty;
                selectedGameValue = string.Empty;
            }
            else
            {
                TempData["Error"] = attemptResult.ErrorMessage;
            }
            return returnJson ? (ActionResult)Json(TempData["Error"]) : View(new RecordingsAddViewModel {GamesOptions = recentGamesOptions, DefaultVideoIdentifier = videoIdentifier, DefaultGameOptionValue = selectedGameValue});
        }

        public AddResult AttemptAdd(double gameStartTimeSeconds, string videoIdentifier, IOrderedEnumerable<IGame> recentGames, List<SelectListItem> recentGamesOptions)
        {
            var result = new AddResult();
            if (gameStartTimeSeconds > 0)
            {
                var recentGame = recentGames.SingleOrDefault(x => x.StartTimeSeconds.Equals(gameStartTimeSeconds));
                if (!string.IsNullOrWhiteSpace(videoIdentifier))
                {
                    if (recentGame != null)
                    {
                        var bkaData = _bkaDataGetter.Get(PlayerId);
                        if (!string.IsNullOrWhiteSpace(bkaData?.Bka))
                        {
                            var videoTimespan = TimeSpan.Zero;
                            try
                            {
                                string json;
                                using (var wc = new WebClient())
                                {
                                    json = wc.DownloadString($"https://www.googleapis.com/youtube/v3/videos?part=contentDetails&id={videoIdentifier}&key=AIzaSyAZcXvaq-g6QyPjDvIWJQfHxRiUrjarOpA");
                                    // title: https://www.googleapis.com/youtube/v3/videos?id=UtCwUo7XI7w&key=AIzaSyAZcXvaq-g6QyPjDvIWJQfHxRiUrjarOpA&fields=items(snippet(title))&part=snippet and http://stackoverflow.com/a/28028528/116895
                                }
                                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                                dynamic parsedDuration = parsedJson.items[0].contentDetails.duration;
                                var videoDuration = Convert.ToString(parsedDuration);
                                videoTimespan = XmlConvert.ToTimeSpan(videoDuration);
                            }
                            catch (Exception e)
                            {
                                result.ErrorMessage = $"Unable to determine {videoIdentifier} duration. Is the upload finished processing and published?";
                            }
                            if (videoTimespan.TotalSeconds > 0)
                            {
                                var expectedDurationInSeconds = recentGame.DurationInSeconds + PREGAME_COUNTDOWN_EXPECTED_DURATION_IN_SECONDS + POSTGAME_PREREADYROOM_EXPECTED_DURATION_IN_SECONDS;
                                var minimumRequiredVideoDurationInSeconds = expectedDurationInSeconds - 11;
                                var maximumAllowedVideoDurationInSeconds = expectedDurationInSeconds + 2;
                                if (videoTimespan.TotalSeconds >= minimumRequiredVideoDurationInSeconds && videoTimespan.TotalSeconds <= maximumAllowedVideoDurationInSeconds)
                                {
                                    _gameRecordingAdder.Add(recentGame.ServerName, recentGame.StartTimeSeconds, PlayerId, videoIdentifier);
                                    var hasReceivedKarmaForThisGame = _gameRecordingKarmaChecker.HasReceivedKarma("ns2", recentGame.ServerName, recentGame.StartTimeSeconds, PlayerId);
                                    if (!hasReceivedKarmaForThisGame)
                                    {
                                        var gameRecordingAddKarmaDeltaName = "AddReplay";
                                        var gameRecordingAddKarmaDelta = 3;
                                        _gameRecordingKarmaAdder.Add("ns2", recentGame.ServerName, recentGame.StartTimeSeconds, PlayerId, gameRecordingAddKarmaDeltaName, gameRecordingAddKarmaDelta);
                                        _karmaAdder.Add("ns2", PlayerId, gameRecordingAddKarmaDeltaName, gameRecordingAddKarmaDelta);
                                        NotifyCameraAdders(recentGame, bkaData, Request);
                                    }
                                }
                                else
                                {
                                    var minimumTimespan = TimeSpan.FromSeconds(minimumRequiredVideoDurationInSeconds).ToString(@"mm\:ss");
                                    var maximumTimespan = TimeSpan.FromSeconds(maximumAllowedVideoDurationInSeconds).ToString(@"mm\:ss");
                                    result.ErrorMessage = $"{videoIdentifier} must begin with game's countdown event and be between {minimumTimespan} and {maximumTimespan}.";
                                }
                            }
                            else
                            {
                                result.ErrorMessage = $"Unable to determine {videoIdentifier} duration. Is the upload finished processing?";
                            }
                        }
                        else
                        {
                            result.ErrorMessage = "Your BKA must be set to add recordings.";
                        }
                    }
                    else
                    {
                        result.ErrorMessage = "Game not found.";
                    }
                }
                else
                {
                    result.ErrorMessage = "No video identifier specified.";
                }
            }
            else
            {
                result.ErrorMessage = "No game specified.";
            }
            return result;
        }

        [HttpGet]
        public JsonResult AddApi(double gameStartTimeSeconds, string videoIdentifier)
        {
            var recentGames = GetRecentGames();
            var recentGamesOptions = GetRecentGamesOptions(recentGames).ToList();
            var attemptResult = AttemptAdd(gameStartTimeSeconds, videoIdentifier, recentGames, recentGamesOptions);
            return Json(new Dictionary<string, object> { {"success", attemptResult.Success}, {"msg", attemptResult.ErrorMessage} }, JsonRequestBehavior.AllowGet);
        }

        private void NotifyCameraAdders(IGame game, IBkaData bkaData, HttpRequestBase httpRequestBase)
        {
            var bka = bkaData.Bka;
            var mapName = game.MapName;
            var when = game.Created.ToShortDateString();
            var minutes = Convert.ToInt32(game.DurationInSeconds / 60);
            var message = $@"{bka} has uploaded a new camera for ""{mapName} {when} [{minutes}m]""";
            var gameRecordings = _gameRecordingsGetter.Get(game.ServerName, game.StartTimeSeconds);
            var gameRecordingPlayerIds = gameRecordings.Select(x => x.PlayerId).ToList();
            if (gameRecordingPlayerIds.Count > 1)
            {
                if (ScheduledTaskRegistry.BotTask.B != null)
                {
                    var playerSteamIds = gameRecordingPlayerIds.Select(_playerIdAdapter.Adapt).ToList();
                    var cameraCount = playerSteamIds.Count;
                    var url = Url.Action("Watch", "Replay", new { serverName = game.ServerName, startTimeSeconds = game.StartTimeSeconds }, httpRequestBase.Url.Scheme);
                    message = $@"{message} ({cameraCount} total cameras now)!";
                    var steamMessage = $@"{message} {url} -- If you don't want these Replay notifications, please give that feedback to Wyzcrak ( https://steamcommunity.com/id/wyzcrak ).";
                    playerSteamIds.Where(x => !ReplayNotificationOptOutNs2Ids.Contains(x.AccountID)).ForEach(playerSteamId => { ScheduledTaskRegistry.BotTask.AddDailySteamMessage(new ScheduledTaskRegistry.DailySteamMessage(DateTime.Now, playerSteamId, steamMessage)); });
                }
            }

            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                _serverAdminCommandSender.Send(serverModel.WebAdminBaseUrl, UserName, PlayerId, $"sh_printallconsoles REPLAY {message} -- M > TGNS Portal > Replay", false);
            }
        }

        public ActionResult DeleteRecording(string serverName, double startTimeSeconds, long playerId)
        {
            var game = _gameRecordingsGetter.Get(serverName, startTimeSeconds).SingleOrDefault(x => x.PlayerId.Equals(playerId));
            if (game != null)
            {
                var userMayDelete = game.PlayerId.Equals(PlayerId) || _playerAdminChecker.IsAdmin(PlayerId);
                if (userMayDelete)
                {
                    if (game.PlayerId.Equals(PlayerId))
                    {
                        _karmaAdder.Add("ns2", PlayerId, "RemoveReplay", -4);
                    }
                    _gameRecordingRemover.Remove(serverName, startTimeSeconds, playerId);
                    TempData["Success"] = "Recording removed.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Not authorized to delete.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Game not found.";
            }
            return RedirectToAction("Delete");
        }

        [HttpPost]
        public ActionResult Get(IEnumerable<IDictionary<string, object>> games)
        {
            //// IEnumerable<IDictionary<string, object>> games = new List<IDictionary<string, object>>();
            //var recordedGames = _gameRecordingsGetter.GetRecordedGamesByPlayer(PlayerId).ToList();
            //var playedGames = _playedGamesGetter.Get("ns2", PlayerId).ToList();

            var recorded = new List<IDictionary<string, object>>();
            var played = new List<IDictionary<string, object>>();
            //foreach (var game in games)
            //{
            //    var serverName = Convert.ToString(game["serverName"]);
            //    var startTimeSeconds = Convert.ToDouble(game["startTimeSeconds"]);
            //    if (recordedGames.Any(y => y.ServerName.Equals(serverName) && y.StartTimeSeconds.Equals(startTimeSeconds)))
            //    {
            //        recorded.Add(game);
            //    }
            //    if (playedGames.Any(y => y.ServerName.Equals(serverName) && y.StartTimeSeconds.Equals(startTimeSeconds)))
            //    {
            //        played.Add(game);
            //    }
            //}
            var result = Json(new Dictionary<string,object> { {"success", true}, {"recorded", recorded }, {"played", played} });
            return result;
        }
    }

    public class AddResult
    {
        public string ErrorMessage { get; set; }

        public bool Success => string.IsNullOrWhiteSpace(ErrorMessage);
    }

    public class RecordingsDeleteViewModel 
    {
        public IEnumerable<IDeletableGameRecording> DeletableGameRecordings { get; set; }
    }
}