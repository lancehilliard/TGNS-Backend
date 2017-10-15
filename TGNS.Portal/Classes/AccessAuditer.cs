using MySql.Data.MySqlClient;
using TGNS.Core.Data;

namespace TGNS.Portal.Classes
{
    public interface IAccessAuditer
    {
        void Audit(string hostname, string what, long playerId);
    }

    public class AccessAuditer : DataAccessor, IAccessAuditer
    {
        public AccessAuditer(string connectionString) : base(connectionString)
        {
        }

        public void Audit(string hostname, string what, long playerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO portal_access (Hostname, What, PlayerId) VALUES (@Hostname, @What, @PlayerId);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@Hostname", hostname);
                    command.Parameters.AddWithValue("@What", what);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}