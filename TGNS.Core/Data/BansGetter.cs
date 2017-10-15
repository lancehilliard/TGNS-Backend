using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBansGetter
    {
        IEnumerable<IBan> Get(string realmName);
    }

    public class BansGetter : DataAccessor, IBansGetter
    {
        public BansGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IBan> Get(string realmName)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand {Connection = connection})
                {
                    command.CommandText = "SELECT BanPlayerId, BanPlayerName, BanDurationInSeconds, BanUnbanTimeInSeconds, BanCreatorName, BanReason, RowCreated, RowUpdated FROM bans WHERE BanRealm=@BanRealm;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BanRealm", realmName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var unbanTime = reader.GetInt64("BanUnbanTimeInSeconds");
                            var playerName = reader.GetString("BanPlayerName");
                            var creatorName = reader.GetString("BanCreatorName");
                            var durationInSeconds = reader.GetInt64("BanDurationInSeconds");
                            var reason = reader.GetString("BanReason");
                            var playerId = reader.GetInt64("BanPlayerId");
                            var created = reader.GetDateTime("RowCreated");
                            var lastModified = reader.GetDateTime("RowUpdated");
                            var ban = new Ban(unbanTime, playerName, creatorName, durationInSeconds, reason, playerId, created, lastModified);
                            yield return ban;
                        }
                    }
                }
            }
        }
    }
}