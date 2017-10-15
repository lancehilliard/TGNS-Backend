using System;

namespace TGNS.Core.Domain
{
    public interface IApproval
    {
        long SourcePlayerId { get; }
        long TargetPlayerId { get; }
        string ServerName { get; }
        double StartTimeSeconds { get; }
        string Reason { get; }
        string Realm { get; }
        DateTime Created { get; }
        DateTime LastModified { get; }
    }

    public class Approval : IApproval
    {
        public Approval(long sourcePlayerId, long targetPlayerId, string serverName, double startTimeSeconds, string reason, string realm, DateTime created, DateTime lastModified)
        {
            SourcePlayerId = sourcePlayerId;
            TargetPlayerId = targetPlayerId;
            ServerName = serverName;
            StartTimeSeconds = startTimeSeconds;
            Reason = reason;
            Realm = realm;
            Created = created;
            LastModified = lastModified;
        }

        public long SourcePlayerId { get; private set; }
        public long TargetPlayerId { get; private set; }
        public string ServerName { get; private set; }
        public double StartTimeSeconds { get; private set; }
        public string Reason { get; private set; }
        public string Realm { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastModified { get; private set; }
    }
}
