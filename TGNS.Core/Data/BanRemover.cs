using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IBanRemover
    {
        void Remove(string realmName, long playerId);
    }

    public class BanRemover : DataAccessor, IBanRemover
    {
        public BanRemover(string connectionString) : base(connectionString)
        {
        }

        public void Remove(string realmName, long playerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "DELETE FROM bans WHERE BanRealm=@BanRealm and BanPlayerId=@PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BanRealm", realmName);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}