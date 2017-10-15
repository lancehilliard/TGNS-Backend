using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IGameRecordingKarmaChecker
    {
        bool HasReceivedKarma(string realmName, string serverName, double startTimeSeconds, long playerId);
    }

    public class GameRecordingKarmaChecker : DataAccessor, IGameRecordingKarmaChecker
    {
        public GameRecordingKarmaChecker(string connectionString) : base(connectionString)
        {
        }

        public bool HasReceivedKarma(string realmName, string serverName, double startTimeSeconds, long playerId)
        {
            long count;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT count(1) as Count FROM games_recordings_karma WHERE Realm = @RealmName and ServerName = @ServerName and StartTimeSeconds = @StartTimeSeconds and KarmaPlayerId = @KarmaPlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RealmName", realmName);
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    command.Parameters.AddWithValue("@KarmaPlayerId", playerId);
                    count = (long)command.ExecuteScalar();
                }
            }
            var result = count > 0;
            return result;
        }
    }
}