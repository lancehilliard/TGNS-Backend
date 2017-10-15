using System;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IPlayerIdGetter
    {
        long Get(string username);
    }

    public class PlayerIdGetter : DataAccessor, IPlayerIdGetter
    {
        public PlayerIdGetter(string connectionString) : base(connectionString)
        {
        }

        public long Get(string username)
        {
            var result = default(long);

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT PlayerId FROM portal_users WHERE PlayerName = @PlayerName;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerName", username);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt64("PlayerId");
                        }
                    }
                }
            }
            if (result == default(long))
            {
                throw new Exception("User not found.");
            }

            return result;
        }
    }
}