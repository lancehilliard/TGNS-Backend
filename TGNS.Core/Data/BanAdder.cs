using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IBanAdder
    {
        void Add(long targetPlayerId, string reason, long sourcePlayerId, string sourcePlayerName);
    }

    public class BanAdder : DataAccessor, IBanAdder
    {
        public BanAdder(string connectionString) : base(connectionString)
        {
        }

        public void Add(long targetPlayerId, string reason, long sourcePlayerId, string sourcePlayerName)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"INSERT INTO bans (BanRealm, BanPlayerId, BanPlayerName, BanDurationInSeconds, BanUnbanTimeInSeconds, BanCreatorName, BanCreatorId, BanReason, BanIssuedTimeInSeconds) VALUES (@BanRealm, @BanPlayerId, '', 0, 0, @BanCreatorName, @BanCreatorId, @BanReason, UNIX_TIMESTAMP(CURRENT_TIME));";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BanRealm", "ns2");
                    command.Parameters.AddWithValue("@BanPlayerId", targetPlayerId);
                    command.Parameters.AddWithValue("@BanReason", reason);
                    command.Parameters.AddWithValue("@BanCreatorName", sourcePlayerName);
                    command.Parameters.AddWithValue("@BanCreatorId", sourcePlayerId);
                    command.ExecuteNonQuery();
                }
            }

        }
    }
}