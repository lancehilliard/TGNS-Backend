using System;
using TGNS.Core.Messaging;

namespace TGNS.Core.Domain
{
    public interface IPushLog
    {
        string Realm { get; }
        long PlayerId { get; }
        string PlatformName { get; }
        PushSummary Summary { get; }
        DateTime Created { get; }
        DateTime LastModified { get; }
    }

    public class PushLog : IPushLog
    {
        public PushLog(string realm, long playerId, string platformName, PushSummary summary, DateTime created, DateTime lastModified)
        {
            Realm = realm;
            PlayerId = playerId;
            PlatformName = platformName;
            Summary = summary;
            Created = created;
            LastModified = lastModified;
        }

        public string Realm { get; private set; }
        public long PlayerId { get; private set; }
        public string PlatformName { get; private set; }
        public PushSummary Summary { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastModified { get; private set; }
    }
}