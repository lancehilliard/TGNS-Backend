using System;

namespace TGNS.Core.Data
{
    public interface IBkaData
    {
        string Bka { get; }
        long PlayerSetGmtInSeconds { get; }
        string PlayerSetGmtDisplay { get; }
        long PlayerId { get; set; }
    }

    public class BkaData : IBkaData
    {
        public BkaData(long playerId, string bka, long playerSetGmtInSeconds)
        {
            PlayerId = playerId;
            Bka = bka;
            PlayerSetGmtInSeconds = playerSetGmtInSeconds;
        }

        public long PlayerId { get; set; }
        public string Bka { get; private set; }
        public long PlayerSetGmtInSeconds { get; private set; }

        public string PlayerSetGmtDisplay
        {
            get
            {
                var playerSetGmt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(PlayerSetGmtInSeconds);
                var result = string.Format("{0} GMT", playerSetGmt.ToString("yyyy-MM-dd HH:mm:ss"));
                return result;
            }
        }
    }
}