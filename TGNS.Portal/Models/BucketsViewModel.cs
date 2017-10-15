using System.Collections.Generic;
using TGNS.Core.Domain;

namespace TGNS.Portal.Models
{
    public class BucketsViewModel
    {
        public BucketsViewModel(IEnumerable<IBucketPlayer> commPlayers, IEnumerable<IBucketPlayer> bestPlayers, IEnumerable<IBucketPlayer> betterPlayers, IEnumerable<IBucketPlayer> goodPlayers)
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

        public IEnumerable<IBucketPlayer> RecentPlayers { get; set; }
        public IEnumerable<string> Logs { get; set; }
    }
}