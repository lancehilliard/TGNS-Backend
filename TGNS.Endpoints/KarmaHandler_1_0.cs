using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class KarmaHandler_1_0 : Handler
    {
        private IKarmaAdder _karmaAdder;

        public KarmaHandler_1_0()
        {
            _karmaAdder = new KarmaAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["d"]);
            return result;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var playerId = request["i"];
            var deltaName = request["n"];
            var delta = request["d"];
            _karmaAdder.Add(realmName, Convert.ToInt64(playerId), deltaName, Convert.ToDecimal(delta));
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var karma = 0;
            var playerId = request["i"];
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "SELECT COALESCE(SUM(KarmaDelta),0) as KarmaSum from karma where KarmaPlayerId = @PlayerId and KarmaRealm = @KarmaRealm;";
                command.Prepare();
                command.Parameters.AddWithValue("@KarmaRealm", realmName);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        karma = reader.GetInt32("KarmaSum");
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", karma } });
            return result;
        }
    }
}