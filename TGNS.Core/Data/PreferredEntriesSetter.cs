using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IPreferredEntriesSetter
    {
        void Set(string realmName, IEnumerable<IPreferredEntry> preferredEntries);
    }

    public class PreferredEntriesSetter : DataAccessor, IPreferredEntriesSetter
    {
        public PreferredEntriesSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(string realmName, IEnumerable<IPreferredEntry> preferredEntries)
        {
            var preferredsToPersist = preferredEntries.Select(blacklistEntry => new Dictionary<string, object> {{"id", blacklistEntry.PlayerId}, {"plugin", blacklistEntry.PluginName}});
            var json = JsonConvert.SerializeObject(new Dictionary<string, object> { { "preferreds", preferredsToPersist } });
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"update data set datavalue=@PreferredsJson where datatypename = 'preferred' and datarealm = @DataRealm;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PreferredsJson", json);
                    command.Parameters.AddWithValue("@DataRealm", realmName);
                    command.ExecuteNonQuery();
                }
            }
        } 
    }
}
