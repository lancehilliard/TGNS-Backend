using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class GuardiansHandler_1_0 : Handler
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
            var data = new List<long>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"select DataRecordId from data where datatypename = 'guardian' and datavalue like '%true%' and datarealm = @DataRealm";
                command.Prepare();
                command.Parameters.AddWithValue("@DataRealm", realmName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader.GetInt64("DataRecordId"));
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;
        }
    }
}