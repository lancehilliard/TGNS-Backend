using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGNS.Core.Data
{
    public interface IReplayVideoStatisticsGetter
    {
        IEnumerable<VideoStatistics> GetStatistics(IEnumerable<string> videoIds);
    }

    public class ReplayVideoStatisticsGetter : IReplayVideoStatisticsGetter
    {
        public IEnumerable<VideoStatistics> GetStatistics(IEnumerable<string> videoIds)
        {
            var videoIdsList = videoIds.ToList();
            for (var i = 0; i < videoIdsList.Count; i=i+50)
            {
                var queryVideoIds = string.Join(",", videoIdsList.Skip(i).Take(50));
                var queryJson = new WebClient().DownloadString($"https://www.googleapis.com/youtube/v3/videos?part=contentDetails,statistics&id={queryVideoIds}&key=AIzaSyDnFN1e2rEVgoDyzXbYeGKlwhZd9WukoaM");
                var queryResult = JsonConvert.DeserializeObject<IDictionary<string, object>>(queryJson);
                var itemsArray = (JArray)queryResult["items"];
                var items = itemsArray.ToObject<IEnumerable<Dictionary<string, object>>>();
                foreach (var item in items)
                {
                    var id = (string)item["id"];
                    var statistics = (JObject)item["statistics"];
                    var viewCount = Convert.ToInt64(statistics["viewCount"]);
                    var likeCount = Convert.ToInt64(statistics["likeCount"]);
                    var dislikeCount = Convert.ToInt64(statistics["dislikeCount"]);
                    var favoriteCount = Convert.ToInt64(statistics["favoriteCount"]);
                    var commentCount = Convert.ToInt64(statistics["commentCount"]);
                    yield return new VideoStatistics(id, viewCount, likeCount, dislikeCount, favoriteCount, commentCount);
                }
            }
        }
    }

    public class VideoStatistics
    {
        public VideoStatistics(string videoIdentifier, long viewCount, long likeCount, long dislikeCount, long favoriteCount, long commentCount)
        {
            VideoIdentifier = videoIdentifier;
            ViewCount = viewCount;
            LikeCount = likeCount;
            DislikeCount = dislikeCount;
            FavoriteCount = favoriteCount;
            CommentCount = commentCount;
        }

        public string VideoIdentifier { get; private set; }
        public long ViewCount { get; private set; }
        public long LikeCount { get; private set; }
        public long DislikeCount { get; private set; }
        public long FavoriteCount { get; private set; }
        public long CommentCount { get; private set; }
    }
}