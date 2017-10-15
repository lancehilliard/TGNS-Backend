using System;

namespace TGNS.Core.Domain
{
    public interface ILap
    {
        long PlayerId { get; }
        string TrackId { get; }
        DateTime When { get; }
        string BuildNumber { get; }
        decimal Seconds { get; }
        string ClassName { get; }
    }

    public class Lap : ILap
    {
        public Lap(long playerId, string trackId, DateTime @when, string buildNumber, decimal seconds, string className)
        {
            PlayerId = playerId;
            TrackId = trackId;
            When = when;
            BuildNumber = buildNumber;
            Seconds = seconds;
            ClassName = className;
        }

        public long PlayerId { get; private set; }
        public string TrackId { get; private set; }
        public DateTime When { get; private set; }
        public string BuildNumber { get; private set; }
        public decimal Seconds { get; private set; }
        public string ClassName { get; private set; }
    }
}
