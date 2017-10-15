using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IGameRecordingRemover
    {
        void Remove(string serverName, double startTimeSeconds, long playerId);
    }

    public class GameRecordingRemover : DataAccessor, IGameRecordingRemover
    {
        public GameRecordingRemover(string connectionString) : base(connectionString)
        {
        }
        public void Remove(string serverName, double startTimeSeconds, long playerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "DELETE FROM games_recordings WHERE ServerName = @ServerName AND StartTimeSeconds = @StartTimeSeconds AND PlayerId = @PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}