using System.Configuration;
using MySql.Data.MySqlClient;

namespace TGNS.Portal.Classes
{
    public interface IPortalUserCreator
    {
        void Create(long ns2Id, string username);
    }

    public class PortalUserCreator : IPortalUserCreator
    {
        public void Create(long ns2Id, string username)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO portal_users (PlayerName, PlayerId) VALUES (@PlayerName, @PlayerId) ON DUPLICATE KEY UPDATE RowUpdated=CURRENT_TIMESTAMP;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerName", username);
                    command.Parameters.AddWithValue("@PlayerId", ns2Id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}