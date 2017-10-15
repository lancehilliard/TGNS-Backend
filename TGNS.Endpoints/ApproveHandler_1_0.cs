using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using TGNS.Core.Data;
using TGNS.Core.Domain;

namespace TGNS.Endpoints
{
    public class ApproveHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["a"]);
            return result;        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var sourcePlayerId = Convert.ToInt64(request["i"]);
            var targetPlayerId = Convert.ToInt64(request["a"]);
            var reason = Convert.ToString(request["re"]);
            var recentTargetPlayerIds = new List<long>();
            var recentTotalApprovals = 0;

            command.CommandText = "SELECT DISTINCT TargetPlayerId FROM approvals WHERE SourcePlayerId = @SourcePlayerId AND RowCreated > DATE_SUB(NOW(), INTERVAL 1 HOUR) ORDER BY RowCreated DESC LIMIT 4;";
            command.Prepare();
            command.Parameters.AddWithValue("@SourcePlayerId", sourcePlayerId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    recentTargetPlayerIds.Add(reader.GetInt64("TargetPlayerId"));
                }
            }
            if (recentTargetPlayerIds.Contains(targetPlayerId))
            {
                throw new Exception("Too many recent approvals for this player.");
            }

            command.CommandText = "SELECT COUNT(1) as Count FROM approvals WHERE SourcePlayerId = @SourcePlayerId AND RowCreated > DATE_SUB(NOW(), INTERVAL 1 HOUR);";
            command.Prepare();
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@SourcePlayerId", sourcePlayerId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    recentTotalApprovals = reader.GetInt32("Count");
                }
            }
            if (recentTotalApprovals >= 15)
            {
                throw new Exception("Too many recent approvals.");
            }

            var serverName = request["s"];
            var startTimeSeconds = Convert.ToDouble(request["t"]);
            command.CommandText = "INSERT INTO approvals (ServerName, StartTimeSeconds, SourcePlayerId, TargetPlayerId, Realm, Reason) VALUES (@ServerName, @StartTimeSeconds, @SourcePlayerId, @TargetPlayerId, @Realm, @Reason);";
            command.Prepare();
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Realm", realmName);
            command.Parameters.AddWithValue("@ServerName", serverName);
            command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
            command.Parameters.AddWithValue("@SourcePlayerId", sourcePlayerId);
            command.Parameters.AddWithValue("@TargetPlayerId", targetPlayerId);
            command.Parameters.AddWithValue("@Reason", reason);
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            string result;
            var timeInDays = request["t"];
            var playerId = request["i"];
            if (playerId != null)
            {
                var count = 0;
                using (var command = new MySqlCommand {Connection = connection})
                {
                    command.CommandText =
                        "SELECT count(distinct sourceplayerid) as ApprovalsCount FROM approvals WHERE rowcreated > DATE_SUB(CURDATE(), INTERVAL @TimeInDays DAY) AND targetplayerid = @PlayerId AND realm = @Realm";
                    command.Prepare();
                    command.Parameters.AddWithValue("@Realm", realmName);
                    command.Parameters.AddWithValue("@TimeInDays", timeInDays);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = reader.GetInt32("ApprovalsCount");
                        }
                    }
                }
                result = JsonConvert.SerializeObject(new Dictionary<string, object> {{"success", true}, {"result", count}});
            }
            else
            {
                var approvalsGetter = new ApprovalsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                var allApprovals = approvalsGetter.Get(realmName);
                var recentApprovals = allApprovals.Where(x=>x.Created > DateTime.Now.AddDays(-1 * Convert.ToInt32(timeInDays))).ToList();
                var approvalCounts = recentApprovals.Select(x=>x.TargetPlayerId).Distinct().ToDictionary(x=>x, x=>recentApprovals.Count(y => y.TargetPlayerId.Equals(x)));
                result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", approvalCounts } });
            }
            return result;
        }
    }
}