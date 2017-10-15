using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints.Badges
{
    public class PlayerBadgesHandler_1_0 : Handler
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
            var badges = new List<Dictionary<string, object>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "SELECT p.BadgeName, p.RowCreated AS BadgeEarnedWhen, b.BadgeDisplayName, b.BadgeDescription, l.LevelName FROM achievements_badges_players p INNER JOIN achievements_badges b on p.AchievementsRealm = b.AchievementsRealm and p.BadgeName = b.BadgeName INNER JOIN achievements_badges_levels l on b.BadgeLevelID = l.LevelID WHERE b.AchievementsRealm = @AchievementsRealm AND p.PlayerId = @PlayerId;";
                command.Prepare();
                command.Parameters.AddWithValue("@AchievementsRealm", realmName);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var badge = new Dictionary<string, object>();
                        badge.Add("Name", reader.GetString("BadgeName"));
                        badge.Add("EarnedWhen", reader.GetDateTime("BadgeEarnedWhen"));
                        badge.Add("DisplayName", reader.GetDateTime("BadgeDisplayName"));
                        badge.Add("Description", reader.GetDateTime("BadgeDescription"));
                        badge.Add("LevelName", reader.GetDateTime("LevelName"));
                        badges.Add(badge);
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", badges } });
            return result;
        }
    }
}