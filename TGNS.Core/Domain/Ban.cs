using System;

namespace TGNS.Core.Domain
{
    public interface IBan
    {
        long UnbanTime { get; }
        string PlayerName { get; }
        string CreatorName { get; }
        long DurationInSeconds { get; }
        string Reason { get; }
        long PlayerId { get; }
        DateTime Created { get; }
        DateTime LastModified { get; }
    }
    
    public class Ban : IBan
    {
        public Ban(long unbanTime, string playerName, string creatorName, long durationInSeconds, string reason, long playerId, DateTime created, DateTime lastModified)
        {
            UnbanTime = unbanTime;
            PlayerName = playerName;
            CreatorName = creatorName;
            DurationInSeconds = durationInSeconds;
            Reason = reason;
            PlayerId = playerId;
            Created = created;
            LastModified = lastModified;
        }

        public long UnbanTime { get; private set; }
        public string PlayerName { get; private set; }
        public string CreatorName { get; private set; }
        public long DurationInSeconds { get; private set; }
        public string Reason { get; private set; }
        public long PlayerId { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastModified { get; private set; }
    }
}