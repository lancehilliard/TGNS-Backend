using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class RecentCaptainsHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            return true;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var recentCaptainPlayerIds = new List<long>();
            var recentPlayerPlayerIds = new List<long>();
            var serverName = request["n"];
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "select distinct playerid, captain from games_players gp inner join games g on g.servername = gp.servername and g.starttimeseconds = gp.StartTimeSeconds and g.CaptainsMode = 1 and g.realm = 'ns2' and g.gamemode = 'ns2'  and g.servername = @ServerName  and g.starttimeseconds = (select max(starttimeseconds) from games where CaptainsMode = 1 and realm = 'ns2' and gamemode = 'ns2' and servername = @ServerName) order by gp.rowcreated desc limit 16";
                command.Prepare();
                command.Parameters.AddWithValue("@ServerName", serverName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var playerId = reader.GetInt64("playerid");
                        recentPlayerPlayerIds.Add(reader.GetInt64("playerid"));
                        if (reader.GetBoolean("captain"))
                        {
                            recentCaptainPlayerIds.Add(playerId);
                        }
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "recentcaptains", recentCaptainPlayerIds }, { "recentplayers", recentPlayerPlayerIds } });
            return result;
        }
    }
}