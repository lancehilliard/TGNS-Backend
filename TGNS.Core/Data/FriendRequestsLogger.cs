using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IFriendRequestsLogger
    {
        void Log(string botName, long playerId);
    }

    public class FriendRequestsLogger : DataAccessor, IFriendRequestsLogger
    {
        public FriendRequestsLogger(string connectionString) : base(connectionString)
        {
        }

        public void Log(string botName, long playerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO `tgns`.`chatbots_friendrequests` (`BotName`, `TargetPlayerId`) VALUES (@BotName, @TargetPlayerId);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BotName", botName);
                    command.Parameters.AddWithValue("@TargetPlayerId", playerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}