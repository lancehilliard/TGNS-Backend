namespace TGNS.Core.Domain
{
    public interface IBlacklistEntry
    {
        long PlayerId { get; }
        string From { get; }
    }

    public class BlacklistEntry : IBlacklistEntry
    {
        public BlacklistEntry(long playerId, string @from)
        {
            PlayerId = playerId;
            From = @from;
        }

        public long PlayerId { get; private set; }
        public string From { get; private set; }

        protected bool Equals(BlacklistEntry other)
        {
            return PlayerId == other.PlayerId && string.Equals(From, other.From);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BlacklistEntry) obj);
        }
    }
}