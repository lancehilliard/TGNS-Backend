using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBucketsPlayerRemover
    {
        void RemoveCommander(long playerId);
        void RemoveBestPlayer(long playerId);
        void RemoveBetterPlayer(long playerId);
        void RemoveGoodPlayer(long playerId);
    }

    public class BucketsPlayerRemover : DataAccessor, IBucketsPlayerRemover
    {
        private readonly IBucketsGetter _bucketsGetter;
        private readonly IBucketsSetter _bucketsSetter;

        public BucketsPlayerRemover(string connectionString)
            : base(connectionString)
        {
            _bucketsGetter = new BucketsGetter(connectionString);
            _bucketsSetter = new BucketsSetter(connectionString);
        }

        public void RemoveCommander(long playerId)
        {
            var buckets = _bucketsGetter.Get();
            var commPlayers = buckets.CommPlayers.Where(x => x.Id != playerId);
            _bucketsSetter.Set(new Buckets(commPlayers, buckets.BestPlayers, buckets.BetterPlayers, buckets.GoodPlayers));
        }

        public void RemoveBestPlayer(long playerId)
        {
            var buckets = _bucketsGetter.Get();
            var bestPlayers = buckets.BestPlayers.Where(x => x.Id != playerId);
            _bucketsSetter.Set(new Buckets(buckets.CommPlayers, bestPlayers, buckets.BetterPlayers, buckets.GoodPlayers));
        }

        public void RemoveBetterPlayer(long playerId)
        {
            var buckets = _bucketsGetter.Get();
            var betterPlayers = buckets.BetterPlayers.Where(x => x.Id != playerId);
            _bucketsSetter.Set(new Buckets(buckets.CommPlayers, buckets.BestPlayers, betterPlayers, buckets.GoodPlayers));
        }

        public void RemoveGoodPlayer(long playerId)
        {
            var buckets = _bucketsGetter.Get();
            var goodPlayers = buckets.GoodPlayers.Where(x => x.Id != playerId);
            _bucketsSetter.Set(new Buckets(buckets.CommPlayers, buckets.BestPlayers, buckets.BetterPlayers, goodPlayers));
        }
    }
}
