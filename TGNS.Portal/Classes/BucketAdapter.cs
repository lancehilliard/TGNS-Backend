using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IBucketAdapter
    {
        BucketsViewModel Adapt(IBuckets buckets);
    }

    public class BucketAdapter : IBucketAdapter
    {
        public BucketsViewModel Adapt(IBuckets buckets)
        {
            var result = new BucketsViewModel(buckets.CommPlayers.OrderBy(x => x.Name.Trim()), buckets.BestPlayers.OrderBy(x => x.Name.Trim()), buckets.BetterPlayers.OrderBy(x => x.Name.Trim()), buckets.GoodPlayers.OrderBy(x => x.Name.Trim()));
            return result;
        }
    }
}