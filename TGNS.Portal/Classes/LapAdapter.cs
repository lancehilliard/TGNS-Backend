using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Controllers;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface ILapAdapter
    {
        IEnumerable<LapDisplayModel> AdaptForBka(IEnumerable<ILap> laps, IEnumerable<IBkaData> bkaDatas, IEnumerable<ITrack> tracks);
        IEnumerable<LapDisplayModel> Adapt(IEnumerable<ILap> laps, IEnumerable<ITrack> tracks);
    }

    public class LapAdapter : ILapAdapter
    {
        private readonly LapDurationFormatter _lapDurationFormatter;

        public LapAdapter()
        {
            _lapDurationFormatter = new LapDurationFormatter();
        }

        public IEnumerable<LapDisplayModel> AdaptForBka(IEnumerable<ILap> laps, IEnumerable<IBkaData> bkaDatas, IEnumerable<ITrack> tracks)
        {
            var bkaLaps = laps.Where(x => bkaDatas.Any(y => y.PlayerId.Equals(x.PlayerId)));
            foreach (var lap in bkaLaps)
            {
                var bka = bkaDatas.Single(x => x.PlayerId.Equals(lap.PlayerId)).Bka;
                var result = Adapt(bka, tracks, lap);
                yield return result;
            }
        }

        public IEnumerable<LapDisplayModel> Adapt(IEnumerable<ILap> laps, IEnumerable<ITrack> tracks)
        {
            return laps.Select(lap => Adapt(string.Empty, tracks, lap));
        }

        private LapDisplayModel Adapt(string bka, IEnumerable<ITrack> tracks, ILap lap)
        {
            var t = tracks.Single(x => x.Id.Equals(lap.TrackId));
            var track = string.Format("{0} ({1} {2})", t.Name, t.MapName, lap.BuildNumber);
            var when = lap.When;
            var duration = _lapDurationFormatter.Format(lap.Seconds);
            var result = new LapDisplayModel(bka, lap.PlayerId, track, @when, duration, lap.ClassName);
            return result;
        }
    }
}