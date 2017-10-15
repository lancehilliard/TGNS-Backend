using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Core.Data
{
    public interface IPlayerAliasesGetter
    {
        IEnumerable<string> Get(long playerId);
        IEnumerable<IPlayerIdentity> Get(string partialPlayerName);
    }

    public class PlayerAliasesGetter : DataAccessor, IPlayerAliasesGetter
    {
        public PlayerAliasesGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<string> Get(long playerId)
        {
            var result = new List<string>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT DataRecordId, DataValue FROM data WHERE DataRealm='ns2' and DataTypeName = 'bka' and DataRecordId = @PlayerId ORDER BY DataUpdated DESC";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bkaJson = reader.GetString("DataValue");
                            bkaJson = bkaJson.Replace(@"\", @"\\");
                            var bkaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bkaJson);
                            var akas = ((Newtonsoft.Json.Linq.JArray)bkaDictionary["AKAs"]).ToObject<IEnumerable<string>>();
                            result.AddRange(akas);
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<IPlayerIdentity> Get(string partialPlayerName)
        {
            var identities = new List<IPlayerIdentity>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"SELECT 
    DataRecordId, DataValue
FROM
    data d
        LEFT JOIN
    (SELECT 
        playerid, MAX(rowcreated) AS MostRecentGame
    FROM
        games_players
    GROUP BY playerid) x ON d.datarecordid = x.playerid
WHERE
    DataRealm = 'ns2'
        AND DataTypeName = 'bka'
        AND DataValue LIKE @PlayerId
ORDER BY ISNULL(x.MostRecentGame), x.MostRecentGame DESC";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", string.Format("%{0}%", partialPlayerName));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("DataRecordId");
                            var bkaJson = reader.GetString("DataValue");
                            bkaJson = bkaJson.Replace(@"\", @"\\");
                            var bkaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bkaJson);
                            var akas = ((Newtonsoft.Json.Linq.JArray)bkaDictionary["AKAs"]).ToObject<IEnumerable<string>>().ToList();
                            if (akas.Any(x => x.ToLower().Contains(partialPlayerName.ToLower())))
                            {
                                if (!akas.First().Equals(bkaDictionary["BKA"]))
                                {
                                    akas.Reverse();
                                }
                                identities.Add(new PlayerIdentity(playerId, akas));
                            }
                        }
                    }
                }
            }
            var result = identities; // .OrderByDescending(x => (x.Aliases.Count(y => y.ToLower().Contains(partialPlayerName.ToLower())) / x.Aliases.Count()) + (x.Aliases.First().ToLower().Contains(partialPlayerName.ToLower()) ? 1 : 0));
            return result;
        }
    }
}
