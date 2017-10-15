using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Models
{
    public class LapDisplayModel
    {
        public LapDisplayModel(string playerName, long playerId, string track, DateTime @when, string duration, string className)
        {
            PlayerName = playerName;
            PlayerId = playerId;
            Track = track;
            When = when;
            Duration = duration;
            ClassName = className;
        }

        public string PlayerName { get; private set; }
        public long PlayerId { get; set; }
        public string Track { get; private set; }
        public DateTime When { get; private set; }
        public string Duration { get; private set; }
        public string ClassName { get; private set; }
    }
}