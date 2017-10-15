using System.Linq;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBucketsPlayerAdder
    {
        void AddCommander(IBucketPlayer player);
        void AddBestPlayer(IBucketPlayer player);
        void AddBetterPlayer(IBucketPlayer player);
        void AddGoodPlayer(IBucketPlayer player);
    }

    public class BucketsPlayerAdder : DataAccessor, IBucketsPlayerAdder
    {
        private readonly IBucketsGetter _bucketsGetter;
        private readonly IBucketsSetter _bucketsSetter;

        public BucketsPlayerAdder(string connectionString) : base(connectionString)
        {
            _bucketsGetter = new BucketsGetter(connectionString);
            _bucketsSetter = new BucketsSetter(connectionString);
        }

        public void AddCommander(IBucketPlayer player)
        {
            var buckets = _bucketsGetter.Get();
            var commPlayers = buckets.CommPlayers.Where(x => x.Id != player.Id).ToList();
            commPlayers.Add(player);
            _bucketsSetter.Set(new Buckets(commPlayers, buckets.BestPlayers, buckets.BetterPlayers, buckets.GoodPlayers));
        }

        public void AddBestPlayer(IBucketPlayer player)
        {
            var buckets = _bucketsGetter.Get();
            var bestPlayers = buckets.BestPlayers.Where(x => x.Id != player.Id).ToList();
            var betterPlayers = buckets.BetterPlayers.Where(x => x.Id != player.Id);
            var goodPlayers = buckets.GoodPlayers.Where(x => x.Id != player.Id);
            bestPlayers.Add(player);
            _bucketsSetter.Set(new Buckets(buckets.CommPlayers, bestPlayers, betterPlayers, goodPlayers));
        }

        public void AddBetterPlayer(IBucketPlayer player)
        {
            var buckets = _bucketsGetter.Get();
            var bestPlayers = buckets.BestPlayers.Where(x => x.Id != player.Id);
            var betterPlayers = buckets.BetterPlayers.Where(x => x.Id != player.Id).ToList();
            var goodPlayers = buckets.GoodPlayers.Where(x => x.Id != player.Id);
            betterPlayers.Add(player);
            _bucketsSetter.Set(new Buckets(buckets.CommPlayers, bestPlayers, betterPlayers, goodPlayers));
        }

        public void AddGoodPlayer(IBucketPlayer player)
        {
            var buckets = _bucketsGetter.Get();
            var bestPlayers = buckets.BestPlayers.Where(x => x.Id != player.Id);
            var betterPlayers = buckets.BetterPlayers.Where(x => x.Id != player.Id);
            var goodPlayers = buckets.GoodPlayers.Where(x => x.Id != player.Id).ToList();
            goodPlayers.Add(player);
            _bucketsSetter.Set(new Buckets(buckets.CommPlayers, bestPlayers, betterPlayers, goodPlayers));
        }
    }
}