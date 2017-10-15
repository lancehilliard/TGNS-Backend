using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IGameRecordingProviderStatisticsGetter
    {
        IEnumerable<VideoStatistics> Get();
    }

    public class GameRecordingProviderStatisticsGetter : DataAccessor, IGameRecordingProviderStatisticsGetter
    {
        public GameRecordingProviderStatisticsGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<VideoStatistics> Get()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT VideoIdentifier, ViewCount, LikeCount, DislikeCount, FavoriteCount, CommentCount FROM games_recordings_provider_statistics;";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var videoIdentifier = reader.GetString("VideoIdentifier");
                            var viewCount = reader.GetInt64("ViewCount");
                            var likeCount = reader.GetInt64("LikeCount");
                            var dislikeCount = reader.GetInt64("DislikeCount");
                            var favoriteCount = reader.GetInt64("FavoriteCount");
                            var commentCount = reader.GetInt64("CommentCount");
                            yield return new VideoStatistics(videoIdentifier, viewCount, likeCount, dislikeCount, favoriteCount, commentCount);
                        }
                    }
                }
            }
        }
    }
}