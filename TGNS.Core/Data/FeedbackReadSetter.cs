using System;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IFeedbackReadSetter
    {
        void Set(long playerId, DateTime created, long readPlayerId);
    }

    public class FeedbackReadSetter : DataAccessor, IFeedbackReadSetter
    {
        public FeedbackReadSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(long playerId, DateTime created, long readPlayerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT IGNORE INTO feedback_read (FeedbackPlayerId, FeedbackCreated, FeedbackReaderPlayerId) VALUES (@PlayerId, @Created, @ReadPlayerId);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.Parameters.AddWithValue("@Created", created);
                    command.Parameters.AddWithValue("@ReadPlayerId", readPlayerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}