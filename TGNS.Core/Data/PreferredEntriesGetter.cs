using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IPreferredEntriesGetter
    {
        IEnumerable<IPreferredEntry> Get(string realmName);
    }

    public class PreferredEntriesGetter : DataAccessor, IPreferredEntriesGetter
    {
        public PreferredEntriesGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IPreferredEntry> Get(string realmName)
        {
            var result = new List<IPreferredEntry>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT DataValue FROM data WHERE DataTypeName = 'preferred' AND DataRealm=@DataRealm;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@DataRealm", realmName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var json = reader.GetString("DataValue");
                            var dataRecord = JsonConvert.DeserializeObject<IDictionary<string,object>>(json);

                            var entries = (JArray)dataRecord["preferreds"];
                            var entryDictionaries = entries.ToObject<IEnumerable<Dictionary<string, object>>>();

                            foreach (var entryDictionary in entryDictionaries)
                            {
                                var playerId = Convert.ToInt64(entryDictionary["id"]);
                                var plugin = Convert.ToString(entryDictionary["plugin"]);
                                result.Add(new PreferredEntry(playerId, plugin));
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
