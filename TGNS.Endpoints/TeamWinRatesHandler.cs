using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class TeamWinRatesHandler_1_0 : Handler
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
            var data = new Dictionary<long, Dictionary<int, double>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"

select PlayerId, MarineWinPercentage, AlienWinPercentage
from (select gp.playerid as PlayerId
			, count(1) as TotalGames
			, coalesce(sum(case when (gp.MarineSeconds > 0 or gp.AlienSeconds > 0) and g.WinningTeamNumber = 1 and gp.MarineSeconds > gp.AlienSeconds then 1 else 0 end) / sum(case when (gp.MarineSeconds > 0 or gp.AlienSeconds > 0) and gp.MarineSeconds > gp.AlienSeconds then 1 else 0 end), 0) as MarineWinPercentage
			, coalesce(sum(case when (gp.MarineSeconds > 0 or gp.AlienSeconds > 0) and g.WinningTeamNumber = 2 and gp.AlienSeconds > gp.MarineSeconds then 1 else 0 end) / sum(case when (gp.MarineSeconds > 0 or gp.AlienSeconds > 0) and gp.AlienSeconds > gp.MarineSeconds then 1 else 0 end), 0) as AlienWinPercentage
		from games_players gp
			inner join games g on g.servername = gp.servername and g.starttimeseconds = gp.starttimeseconds
		where gp.rowcreated > DATE_SUB(CURDATE(), INTERVAL 6 MONTH)
			and g.realm = 'ns2'
			and g.gamemode = 'ns2'
			and g.durationinseconds > 450
		group by gp.playerid) x
where TotalGames > 15

";
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var playerId = reader.GetInt64("PlayerId");
                        var marineWinPercentage = reader.GetDouble("MarineWinPercentage");
                        var alienWinPercentage = reader.GetDouble("AlienWinPercentage");
                        data.Add(playerId, new Dictionary<int, double> { { 1, marineWinPercentage }, { 2, alienWinPercentage } });
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;

        }
    }
}