using MySql.Data.MySqlClient;
using TGNS.Core.Data;

namespace TGNS.Core.Messaging
{
    public interface IMessagePushLogger
    {
        void Log(string realmName, long playerId, string platform, string input, string output, int resultCode, string resultDescription);
    }

    public class MessagePushLogger : DataAccessor, IMessagePushLogger
    {
        public MessagePushLogger(string connectionString) : base(connectionString)
        {
        }

        public void Log(string realmName, long playerId, string platform, string input, string output, int resultCode, string resultDescription) {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO pushes (PushRealm, PushPlayerId, PushPlatform, PushInput, PushOutput, PushResultCode, PushResultDescription) VALUES (@PushRealm, @PushPlayerId, @PushPlatform, @PushInput, @PushOutput, @PushResultCode, @PushResultDescription);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PushRealm", realmName);
                    command.Parameters.AddWithValue("@PushPlayerId", playerId);
                    command.Parameters.AddWithValue("@PushPlatform", platform);
                    command.Parameters.AddWithValue("@PushInput", input);
                    command.Parameters.AddWithValue("@PushOutput", output);
                    command.Parameters.AddWithValue("@PushResultCode", resultCode);
                    command.Parameters.AddWithValue("@PushResultDescription", resultDescription);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}