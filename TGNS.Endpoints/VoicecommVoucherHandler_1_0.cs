using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class VoicecommVoucherHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["v"]);
            return result;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var sourcePlayerId = Convert.ToInt64(request["i"]);
            var targetPlayerId = Convert.ToInt64(request["v"]);
            command.CommandText = "INSERT INTO voicecomm_vouches (SourcePlayerId, TargetPlayerId) VALUES (@SourcePlayerId, @TargetPlayerId);";
            command.Prepare();
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@SourcePlayerId", sourcePlayerId);
            command.Parameters.AddWithValue("@TargetPlayerId", targetPlayerId);
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var timeInHours = request["h"];
            var playerIds = new List<dynamic>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "SELECT distinct SourcePlayerId, TargetPlayerId FROM voicecomm_vouches WHERE rowupdated > DATE_SUB(CURRENT_TIMESTAMP, INTERVAL @TimeInHours HOUR)";
                command.Prepare();
                command.Parameters.AddWithValue("@TimeInHours", timeInHours);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        playerIds.Add(new {SourcePlayerId = reader.GetInt64("SourcePlayerId"), TargetPlayerId = reader.GetInt64("TargetPlayerId")});
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", playerIds } });
            return result;
        }

    }
}