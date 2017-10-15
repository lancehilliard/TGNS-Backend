using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IFeedbacksSetter
    {
        void Set(long playerId, string subject, string body);
    }

    public class FeedbacksSetter : DataAccessor, IFeedbacksSetter
    {
        public FeedbacksSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(long playerId, string subject, string body)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO feedback (FeedbackPlayerId, FeedbackSubject, FeedbackBody) VALUES (@PlayerId, @Subject, @Body);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.Parameters.AddWithValue("@Subject", subject);
                    command.Parameters.AddWithValue("@Body", body);
                    command.ExecuteNonQuery();
                }
            }
        } 
    }
}