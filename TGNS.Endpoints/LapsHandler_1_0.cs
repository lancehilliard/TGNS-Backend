using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class LapsHandler_1_0 : Handler
    {
        private readonly IBkaDataGetter _bkaDataGetter;

        public LapsHandler_1_0()
        {
            _bkaDataGetter = new BkaDataGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString, new BkaDataParser());
        }

        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["s"]);
            return result;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var playerId = Convert.ToInt64(request["i"]);
            var trackId = request["t"];
            var className = request["c"];
            var buildNumber = Convert.ToInt32(request["b"]);
            var seconds = Convert.ToDecimal(request["s"]);
            if (seconds > 0)
            {
                command.CommandText = "INSERT INTO laps (PlayerId, TrackId, BuildNumber, Seconds, ClassName) VALUES (@PlayerId, @TrackId, @BuildNumber, @Seconds, @ClassName);";
                command.Prepare();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@PlayerId", playerId);
                command.Parameters.AddWithValue("@TrackId", trackId);
                command.Parameters.AddWithValue("@BuildNumber", buildNumber);
                command.Parameters.AddWithValue("@Seconds", seconds);
                command.Parameters.AddWithValue("@ClassName", className);
                command.ExecuteNonQuery();
            }
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var trackId = request["t"];
            var buildNumber = request["b"];
            var playerId = request["i"];
            var className = request["c"];
            decimal seconds = 0;
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "select min(seconds) as seconds from laps where trackid = @TrackId and buildNumber = @BuildNumber and playerid = @PlayerId and className = @ClassName;";
                command.Prepare();
                command.Parameters.AddWithValue("@TrackId", trackId);
                command.Parameters.AddWithValue("@BuildNumber", buildNumber);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                command.Parameters.AddWithValue("@ClassName", className);
                var secondsResult = command.ExecuteScalar();
                if (secondsResult != null && secondsResult != DBNull.Value)
                {
                    seconds = Convert.ToDecimal(secondsResult);
                }
            }
            decimal bestSeconds = 0;
            var bestPlayerBkaName = string.Empty;
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "select playerid, min(seconds) as seconds from laps where trackid = @TrackId and buildNumber = @BuildNumber and className = @ClassName group by playerid order by 2 limit 1;";
                command.Prepare();
                command.Parameters.AddWithValue("@TrackId", trackId);
                command.Parameters.AddWithValue("@BuildNumber", buildNumber);
                command.Parameters.AddWithValue("@ClassName", className);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bestPlayerId = reader.GetInt64("playerid");
                        var bkaData = _bkaDataGetter.Get(bestPlayerId);
                        if (!string.IsNullOrWhiteSpace(bkaData?.Bka))
                        {
                            bestPlayerBkaName = bkaData.Bka;
                        }

                        bestSeconds = reader.GetDecimal("seconds");
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> {{"success", true}, {"seconds", seconds}, {"bestSeconds", bestSeconds}, {"bestBkaName", bestPlayerBkaName } });
            return result;
        }
    }
}