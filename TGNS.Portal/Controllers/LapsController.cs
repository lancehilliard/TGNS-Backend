using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class LapsController : AuthenticatedController
    {
        private readonly ITracksGetter _tracksGetter;
        private readonly ILapsGetter _lapsGetter;
        private readonly ILapAdapter _lapAdapter;
        private readonly IBkaDataGetter _bkaDataGetter;

        public LapsController()
        {
            _tracksGetter = new TracksGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _lapsGetter = new LapsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _lapAdapter = new LapAdapter();
            _bkaDataGetter = new BkaDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
        }

        public ViewResult Manage()
        {
            var tracks = _tracksGetter.Get().ToList();
            var laps = _lapsGetter.Get(PlayerId).ToList();
            var lapDisplayModels = _lapAdapter.Adapt(laps, tracks);
            return View(new LapsManageViewModel { Tracks = tracks.Where(x => laps.Any(y => y.TrackId.Equals(x.Id))), Laps = lapDisplayModels });
        }

        public ViewResult Index()
        {
            var tracks = _tracksGetter.Get().ToList();
            var laps = _lapsGetter.Get().ToList();
            var bkaDatas = _bkaDataGetter.Get().ToList();
            var lapDisplayModels = _lapAdapter.AdaptForBka(laps, bkaDatas, tracks);
            return View(new LapsIndexViewModel { Tracks = tracks.Where(x=>laps.Any(y=>y.TrackId.Equals(x.Id))), Laps = lapDisplayModels });
        }

        public ViewResult Help()
        {
            return View();
        }

        public ViewResult Tracks()
        {
            var tracks = _tracksGetter.Get();
            return View(tracks);
        }
    }

    public class LapDurationFormatter
    {
        public string Format(decimal seconds)
        {
            var secondsIntegral = Convert.ToInt32(Math.Floor(seconds));
            var secondsFractional = seconds % secondsIntegral;
            var milliseconds = Convert.ToInt32(secondsFractional * 1000);
            var duration = new TimeSpan(0, 0, 0, secondsIntegral, milliseconds);
            var durationDisplay = string.Format("{0}:{1}:{2}", duration.Minutes.ToString("00"), duration.Seconds.ToString("00"), duration.Milliseconds.ToString("000"));
            var result = duration.TotalMinutes < 60 ? durationDisplay : "Quite Long";
            return result;
        }
    }
}