using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBlacklistEntriesSetter
    {
        void Set(string realmName, IEnumerable<IBlacklistEntry> blacklistEntries);
    }

    public class BlacklistEntriesSetter : DataAccessor, IBlacklistEntriesSetter
    {
        public BlacklistEntriesSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(string realmName, IEnumerable<IBlacklistEntry> blacklistEntries)
        {
            var blacklistsToPersist = blacklistEntries.Select(blacklistEntry => new Dictionary<string, object> {{"id", blacklistEntry.PlayerId}, {"from", blacklistEntry.From}});
            var json = JsonConvert.SerializeObject(new Dictionary<string, object> { { "blacklists", blacklistsToPersist } });
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"update data set datavalue=@BlacklistsJson where datatypename = 'blacklist' and datarealm = @DataRealm;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BlacklistsJson", json);
                    command.Parameters.AddWithValue("@DataRealm", realmName);
                    command.ExecuteNonQuery();
                }
            }
        } 
    }
}