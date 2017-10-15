using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IChatBotsLogLineAdder
    {
        void Add(string botName, string logLevel, string message);
    }

    public class ChatBotsLogLineAdder : DataAccessor, IChatBotsLogLineAdder
    {
        public void Add(string botName, string logLevel, string message)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO chatbots_log (BotName, LogLevel, Message) VALUES (@BotName, @LogLevel, @Message);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BotName", botName);
                    command.Parameters.AddWithValue("@LogLevel", logLevel);
                    command.Parameters.AddWithValue("@Message", message);
                    command.ExecuteNonQuery();
                }
            }
        }

        public ChatBotsLogLineAdder(string connectionString) : base(connectionString)
        {
        }
    }
}