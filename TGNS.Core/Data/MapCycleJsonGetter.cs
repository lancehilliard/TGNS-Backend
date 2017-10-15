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
    public interface IMapCycleJsonGetter
    {
        string Get();
    }

    public class MapCycleJsonGetter : DataAccessor, IMapCycleJsonGetter
    {
        public string Get()
        {
            var result = "{}";
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select PluginConfig from plugins where PluginName = 'MapCycle.json' and PluginRealm = 'ns2';";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetString("PluginConfig");
                        }
                    }
                }
            }
            return result;
        }

        public MapCycleJsonGetter(string connectionString) : base(connectionString)
        {
        }
    }
}
