using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Core.Data
{
    public interface IPlaypalsGetter
    {
        IEnumerable<IPlayer> GetRecent(long playerId);
    }

    public class PlaypalsGetter : DataAccessor, IPlaypalsGetter
    {
        public PlaypalsGetter(string connectionString) : base(connectionString)
        {

        }

        public IEnumerable<IPlayer> GetRecent(long playerId)
        {
            var result = new List<IPlayer>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"SELECT 
    y.playerid as PlayerId, y.count as Count, d.datavalue AS BkaJson
FROM
    (SELECT 
        gp.playerid, COUNT(1) AS count
    FROM
        games_players gp
    INNER JOIN (SELECT DISTINCT
        servername, starttimeseconds
    FROM
        games_players gp
    WHERE
        playerid = @PlayerId) x ON gp.servername = x.servername
        AND gp.starttimeseconds = x.starttimeseconds
        AND gp.rowcreated > DATE_SUB(CURDATE(), INTERVAL 14 DAY)
    WHERE
        playerid <> @PlayerId
    GROUP BY playerid) y
        INNER JOIN
    data d ON d.datarealm = 'ns2'
        AND d.datatypename = 'bka'
        AND d.datarecordid = y.playerid
        AND d.datavalue NOT LIKE '%""BKA"":""""%'
        AND d.datavalue NOT LIKE '%""BKA"": """"%'
ORDER BY y.count DESC";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var palPlayerId = Convert.ToInt64(reader.GetString("PlayerId"));
                            var bkaJson = reader.IsDBNull(reader.GetOrdinal("BkaJson")) ? null : reader.GetString("BkaJson");
                            if (bkaJson != null)
                            {
                                var bkaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bkaJson);
                                var bka = bkaDictionary["BKA"] as string;
                                result.Add(new Player(bka, palPlayerId));
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}