using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class RolesHandler_1_0 : Handler
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
            var playerDatas = new List<Dictionary<string, object>>();
            var playerIdsInput = request["i"];
            var days = request["d"];
            var playerIds = playerIdsInput.Split(',');
            var inClauseParametersSql = string.Join(",", Enumerable.Range(0, playerIds.Length).Select(x => $"@p{x}"));
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = $"SELECT PlayerId, SUM(GorgeSeconds) AS GorgeSeconds, SUM(LerkSeconds) AS LerkSeconds, SUM(FadeSeconds) AS FadeSeconds, SUM(OnosSeconds) AS OnosSeconds, SUM(CASE MarineSeconds > AlienSeconds WHEN TRUE THEN CommanderSeconds ELSE 0 END) AS MarineCommSeconds, SUM(CASE AlienSeconds > MarineSeconds WHEN TRUE THEN CommanderSeconds ELSE 0 END) AS AlienCommSeconds FROM games_players gp INNER JOIN games g ON g.ServerName = gp.ServerName AND g.StartTimeSeconds = gp.StartTimeSeconds WHERE g.Realm = @RealmName and g.GameMode = 'ns2' AND gp.RowCreated > DATE_SUB(CURDATE(), INTERVAL @Days DAY) AND gp.PlayerId IN ({inClauseParametersSql}) AND GAME_QUALIFIES_FOR_ACHIEVEMENTS(g.ServerName, g.StartTimeSeconds) GROUP BY gp.PlayerId;";
                command.Parameters.AddWithValue("@RealmName", realmName);
                command.Parameters.AddWithValue("@Days", days);
                for (var i = 0; i < playerIds.Length; i++)
                {
                    var playerId = playerIds[i];
                    command.Parameters.AddWithValue($"@p{i}", playerId);
                }
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    var decimalColumnNames = new List<string> { "GorgeSeconds", "LerkSeconds", "FadeSeconds", "OnosSeconds", "MarineCommSeconds", "AlienCommSeconds" };
                    while (reader.Read())
                    {
                        var playerData = decimalColumnNames.ToDictionary(columnName => columnName, columnName => reader.GetDecimal(columnName) as object);
                        playerData.Add("KD", 0);
                        playerData.Add("PlayerId", reader.GetInt64("PlayerId"));
                        playerDatas.Add(playerData);
                    }
                }
            }
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = $"SELECT PlayerId, CASE SUM(Deaths) > 0 WHEN TRUE THEN SUM(Kills+Assists)/SUM(Deaths) ELSE 0 END AS KD FROM games_players gp INNER JOIN games g ON g.ServerName = gp.ServerName AND g.StartTimeSeconds = gp.StartTimeSeconds WHERE g.Realm = @RealmName and g.GameMode = 'ns2' AND gp.RowCreated > DATE_SUB(CURDATE(), INTERVAL 30 DAY) AND gp.PlayerId IN ({inClauseParametersSql}) AND gp.MarineSeconds > gp.AlienSeconds AND GAME_QUALIFIES_FOR_ACHIEVEMENTS(g.ServerName, g.StartTimeSeconds) GROUP BY gp.PlayerId;";
                command.Parameters.AddWithValue("@RealmName", realmName);
                command.Parameters.AddWithValue("@Days", days);
                for (var i = 0; i < playerIds.Length; i++)
                {
                    var playerId = playerIds[i];
                    command.Parameters.AddWithValue($"@p{i}", playerId);
                }
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var playerData = playerDatas.FirstOrDefault(x => ((long)x["PlayerId"]) == reader.GetInt64("PlayerId"));
                        if (playerData != null)
                        {
                            playerData["KD"] = reader.GetDecimal("KD");
                        }
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", playerDatas } });
            return result;
        }
    }
}