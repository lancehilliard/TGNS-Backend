using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGNS.Core.Domain
{
    public interface IBuckets
    {
        IEnumerable<IBucketPlayer> CommPlayers { get; }
        IEnumerable<IBucketPlayer> BestPlayers { get; }
        IEnumerable<IBucketPlayer> BetterPlayers { get; }
        IEnumerable<IBucketPlayer> GoodPlayers { get; }
    }

    class Buckets : IBuckets
    {
        public Buckets(IEnumerable<IBucketPlayer> commPlayers, IEnumerable<IBucketPlayer> bestPlayers, IEnumerable<IBucketPlayer> betterPlayers, IEnumerable<IBucketPlayer> goodPlayers)
        {
            CommPlayers = commPlayers;
            BestPlayers = bestPlayers;
            BetterPlayers = betterPlayers;
            GoodPlayers = goodPlayers;
        }

        public IEnumerable<IBucketPlayer> CommPlayers { get; private set; }
        public IEnumerable<IBucketPlayer> BestPlayers { get; private set; }
        public IEnumerable<IBucketPlayer> BetterPlayers { get; private set; }
        public IEnumerable<IBucketPlayer> GoodPlayers { get; private set; }
    }
}
