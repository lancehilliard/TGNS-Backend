using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Models
{
    public class BanViewModel
    {
        public BanViewModel(long unbanTime, string playerName, string creatorName, long durationInSeconds, string reason, long playerId)
        {
            UnbanTime = unbanTime;
            PlayerName = playerName;
            CreatorName = creatorName;
            DurationInSeconds = durationInSeconds;
            Reason = reason;
            PlayerId = playerId;
        }

        public long UnbanTime { get; private set; }
        public string PlayerName { get; private set; }
        public string CreatorName { get; private set; }
        public long DurationInSeconds { get; private set; }
        public string Reason { get; private set; }
        public long PlayerId { get; private set; }
    }
}