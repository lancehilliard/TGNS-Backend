using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class SpecModeHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            return true;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var data = new Dictionary<long, Dictionary<string, object>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"select DataValue from data where datatypename = 'specmode' and datarealm = @DataRealm";
                command.Prepare();
                command.Parameters.AddWithValue("@DataRealm", realmName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dataValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.GetString("DataValue"));
                        var steamId = Convert.ToInt64(dataValue["steamId"]);
                        var specmode = dataValue.ContainsKey("specmode") ? Convert.ToInt32(dataValue["specmode"]) : 0;
                        var specPriority = dataValue.ContainsKey("specpriority") && Convert.ToBoolean(dataValue["specpriority"]);
                        data.Add(steamId, new Dictionary<string, object> { { "specMode", specmode }, { "specPriority", specPriority } });
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;
        }
    }
}