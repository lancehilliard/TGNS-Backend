using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IBadgePlayerGetter
    {
        IEnumerable<IBadgePlayerModel> Get(long playerId);
    }

    public class BadgePlayerGetter : IBadgePlayerGetter
    {
        public IEnumerable<IBadgePlayerModel> Get(long playerId)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT 
    b.BadgeID,
    bl.LevelName,
    BadgeName,
	BadgeDisplayName,
    BadgeDescription,
    bp.RowCreated,
    bp.ShowInGame
FROM
    achievements_badges b
        INNER JOIN
    achievements_badges_levels bl ON b.BadgeLevelID = bl.LevelID
        INNER JOIN
    achievements_badges_players bp ON b.achievementsrealm = bp.achievementsrealm
        AND b.badgeid = bp.badgeid
        AND bp.playerid = @PlayerId
WHERE
    bp.achievementsrealm = 'ns2'
ORDER BY bp.RowCreated DESC, b.BadgeId DESC;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32("BadgeID");
                            var levelName = reader.GetString("LevelName");
                            var name = reader.GetString("BadgeName");
                            var displayName = reader.GetString("BadgeDisplayName");
                            var description = reader.GetString("BadgeDescription");
                            var created = reader.GetDateTime("RowCreated");
                            var showInGame = reader.GetBoolean("ShowInGame");
                            var badge = new BadgePlayerModel(id, levelName, name, displayName, description, created, showInGame);
                            yield return badge;
                        }
                    }
                }
            }
        } 
    }
}