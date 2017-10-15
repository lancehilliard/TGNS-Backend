using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBlacklistEntriesGetter
    {
        IEnumerable<IBlacklistEntry> Get(string realmName);
    }

    public class BlacklistEntriesGetter : DataAccessor, IBlacklistEntriesGetter
    {
        public BlacklistEntriesGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IBlacklistEntry> Get(string realmName)
        {
            var result = new List<IBlacklistEntry>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT DataValue FROM data WHERE DataTypeName = 'blacklist' AND DataRealm=@DataRealm;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@DataRealm", realmName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var json = reader.GetString("DataValue");
                            var dataRecord = JsonConvert.DeserializeObject<IDictionary<string,object>>(json);

                            var entries = (JArray)dataRecord["blacklists"];
                            var entryDictionaries = entries.ToObject<IEnumerable<Dictionary<string, object>>>();

                            foreach (var entryDictionary in entryDictionaries)
                            {
                                var playerId = Convert.ToInt64(entryDictionary["id"]);
                                var from = Convert.ToString(entryDictionary["from"]);
                                result.Add(new BlacklistEntry(playerId, from));
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}