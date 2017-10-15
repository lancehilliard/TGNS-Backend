using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints.Badges
{
    public class MostRecentBadgeHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            return true;
        }

        protected override void Write(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            var playerId = request["i"];
            var mostRecentBadge = new Dictionary<string, object>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "SELECT b.BadgeID, b.BadgeDisplayName FROM achievements_badges b INNER JOIN achievements_badges_players bp ON b.AchievementsRealm = bp.AchievementsRealm AND b.BadgeID = bp.BadgeID AND b.AchievementsRealm = @AchievementsRealm AND bp.PlayerId = @PlayerId INNER JOIN (SELECT MAX(sub_bp.RowCreated) AS MaxRowCreated FROM achievements_badges sub_b INNER JOIN achievements_badges_players sub_bp ON sub_b.AchievementsRealm = sub_bp.AchievementsRealm AND sub_bp.AchievementsRealm = @AchievementsRealm AND sub_bp.PlayerId = @PlayerId AND sub_b.BadgeID = sub_bp.BadgeID) sub ON bp.RowCreated = sub.MaxRowCreated LIMIT 1;";
                command.Prepare();
                command.Parameters.AddWithValue("@AchievementsRealm", realmName);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mostRecentBadge.Add("ID", reader.GetString("BadgeID"));
                        mostRecentBadge.Add("DisplayName", reader.GetString("BadgeDisplayName"));
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", mostRecentBadge } });
            return result;
        }
    }
}