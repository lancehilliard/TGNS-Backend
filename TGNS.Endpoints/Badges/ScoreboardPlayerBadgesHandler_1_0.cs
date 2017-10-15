using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints.Badges
{
    public class ScoreboardPlayerBadgesHandler_1_0 : Handler
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
            var data = new Dictionary<long, List<Dictionary<string, object>>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "SELECT bp.PlayerId, b.BadgeID, b.BadgeDisplayName, b.BadgeDescription FROM achievements_badges b INNER JOIN achievements_badges_players bp on b.AchievementsRealm = bp.AchievementsRealm AND b.BadgeID = bp.BadgeID AND b.AchievementsRealm = @AchievementsRealm WHERE bp.ShowInGame = 1;";
                command.Prepare();
                command.Parameters.AddWithValue("@AchievementsRealm", realmName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var playerId = reader.GetInt64("PlayerId");
                        var badgeId = reader.GetInt32("BadgeID");
                        var badgeDisplayName = reader.GetString("BadgeDisplayName");
                        var badgeDescription = reader.GetString("BadgeDescription");
                        var badge = new Dictionary<string, object>{ {"ID", badgeId}, {"DisplayName", badgeDisplayName}, {"Description", badgeDescription} };
                        var badges = new List<Dictionary<string, object>> { badge };
                        if (data.ContainsKey(playerId))
                        {
                            badges.AddRange(data[playerId]);
                            data.Remove(playerId);
                        }
                        data.Add(playerId, badges);
                    }
                }
            }

            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;
        }
    }
}