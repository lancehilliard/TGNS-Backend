using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Core.Data
{
    public interface IKarmaAdder
    {
        void Add(string realmName, long playerId, string deltaName, decimal delta);
    }

    public class KarmaAdder : DataAccessor, IKarmaAdder
    {
        public void Add(string realmName, long playerId, string deltaName, decimal delta)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand {Connection = connection})
                {
                    command.CommandText = "INSERT INTO karma (KarmaRealm, KarmaPlayerId, KarmaDeltaName, KarmaDelta) VALUES (@KarmaRealm, @KarmaPlayerId, @KarmaDeltaName, @KarmaDelta);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@KarmaRealm", realmName);
                    command.Parameters.AddWithValue("@KarmaPlayerId", playerId);
                    command.Parameters.AddWithValue("@KarmaDeltaName", deltaName);
                    command.Parameters.AddWithValue("@KarmaDelta", delta);
                    command.ExecuteNonQuery();
                }
            }
        }

        public KarmaAdder(string connectionString) : base(connectionString)
        {
        }
    }
}