using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class WraplengthHandler_1_0 : Handler
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
            var data = new Dictionary<long, int>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"select DataValue from data where datatypename = 'wraplength' and datarealm = @DataRealm";
                command.Prepare();
                command.Parameters.AddWithValue("@DataRealm", realmName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dataValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.GetString("DataValue"));
                        var steamId = Convert.ToInt64(dataValue["steamId"]);
                        var percentage = Convert.ToInt32(dataValue["wraplength"]);
                        data.Add(steamId, percentage);
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;
        }
    }
}