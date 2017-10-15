using System;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IGameRecordingAdder
    {
        void Add(string serverName, double startTimeSeconds, long playerId, string videoIdentifier);
    }

    public class GameRecordingAdder : DataAccessor, IGameRecordingAdder
    {
        public GameRecordingAdder(string connectionString) : base(connectionString)
        {
        }

        public void Add(string serverName, double startTimeSeconds, long playerId, string videoIdentifier)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"INSERT INTO games_recordings(ServerName, StartTimeSeconds, PlayerId, VideoIdentifier) VALUES(@ServerName, @StartTimeSeconds, @PlayerId, @VideoIdentifier) ON DUPLICATE KEY UPDATE VideoIdentifier=@VideoIdentifier;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.Parameters.AddWithValue("@VideoIdentifier", videoIdentifier);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}