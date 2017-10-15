using System;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IGameRecordingKarmaAdder
    {
        void Add(string realmName, string serverName, double startTimeSeconds, long playerId, string deltaName, decimal delta);
    }

    public class GameRecordingKarma : DataAccessor, IGameRecordingKarmaAdder
    {
        public GameRecordingKarma(string connectionString) : base(connectionString)
        {
        }

        public void Add(string realmName, string serverName, double startTimeSeconds, long playerId, string deltaName, decimal delta)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"INSERT INTO games_recordings_karma (Realm, ServerName, StartTimeSeconds, KarmaPlayerId, KarmaDeltaName, KarmaDelta) VALUES (@RealmName, @ServerName, @StartTimeSeconds, @KarmaPlayerId, @KarmaDeltaName, @KarmaDelta);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RealmName", realmName);
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    command.Parameters.AddWithValue("@KarmaPlayerId", playerId);
                    command.Parameters.AddWithValue("@KarmaDeltaName", deltaName);
                    command.Parameters.AddWithValue("@KarmaDelta", delta);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
