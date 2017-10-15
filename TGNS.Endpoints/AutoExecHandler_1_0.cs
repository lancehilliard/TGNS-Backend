using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGNS.Endpoints
{
    public class AutoExecHandler_1_0 : Handler
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
            var data = new Dictionary<long, IEnumerable<string>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"select DataValue from data where datatypename = 'autoexec' and datarealm = @DataRealm";
                command.Prepare();
                command.Parameters.AddWithValue("@DataRealm", realmName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dataValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.GetString("DataValue"));
                        var steamId = Convert.ToInt64(dataValue["steamId"]);
                        var commands = ((JArray)dataValue["commands"]).ToObject<IEnumerable<string>>();
                        data.Add(steamId, commands);
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;
        }
    }
}