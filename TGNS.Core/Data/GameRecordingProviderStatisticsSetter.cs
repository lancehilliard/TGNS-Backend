using System.Configuration;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IGameRecordingProviderStatisticsSetter
    {
        void Set(VideoStatistics statistics);
    }

    public class GameRecordingProviderStatisticsSetter : IGameRecordingProviderStatisticsSetter
    {
        public void Set(VideoStatistics statistics)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO games_recordings_provider_statistics (VideoProvider, VideoIdentifier, ViewCount, LikeCount, DislikeCount, FavoriteCount, CommentCount) VALUES ('YouTube', @VideoIdentifier, @ViewCount, @LikeCount, @DislikeCount, @FavoriteCount, @CommentCount) ON DUPLICATE KEY UPDATE ViewCount=@ViewCount, LikeCount=@LikeCount, DislikeCount=@DislikeCount, FavoriteCount=@FavoriteCount, CommentCount=@CommentCount;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@VideoIdentifier", statistics.VideoIdentifier);
                    command.Parameters.AddWithValue("@ViewCount", statistics.ViewCount);
                    command.Parameters.AddWithValue("@LikeCount", statistics.LikeCount);
                    command.Parameters.AddWithValue("@DislikeCount", statistics.DislikeCount);
                    command.Parameters.AddWithValue("@FavoriteCount", statistics.FavoriteCount);
                    command.Parameters.AddWithValue("@CommentCount", statistics.CommentCount);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}