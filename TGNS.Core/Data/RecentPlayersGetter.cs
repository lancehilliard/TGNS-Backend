using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IRecentPlayersGetter
    {
        IEnumerable<IBucketPlayer> Get(string realmName);
    }

    public class RecentPlayersGetter : DataAccessor, IRecentPlayersGetter
    {
        public RecentPlayersGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IBucketPlayer> Get(string realmName)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"select x.count, x.playerid as PlayerId, d.datavalue as DataValue
                        from (select 
                                count(1) AS `count`,
                                `gp`.`PlayerId` AS `playerid`
                            from
                                `games_players` `gp`
                                join `games` `g` ON `g`.`ServerName` = `gp`.`ServerName` and `g`.`StartTimeSeconds` = `gp`.`StartTimeSeconds`
                            where
                                `gp`.`RowCreated` > (curdate() - interval 14 day)
                                    and `g`.`Realm` = @RealmName
                            group by `gp`.`PlayerId`
                        order by 1 desc) x
                        inner join `data` `d` ON `d`.`DataTypeName` = 'bka' and `d`.`DataRecordId` = `x`.`PlayerId` and d.datarealm = @RealmName
                        order by x.count desc";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RealmName", realmName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("PlayerId");
                            var bkaJson = reader.GetString("DataValue");
                            IDictionary<string,object> bkaDictionary = null;
                            IEnumerable<string> akas = null;
                            try
                            {
                                bkaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bkaJson);
                                akas = ((Newtonsoft.Json.Linq.JArray) bkaDictionary["AKAs"]).ToObject<IEnumerable<string>>();
                            }
                            catch (Exception)
                            {
                            }
                            if (bkaDictionary != null)
                            {
                                var playerName = bkaDictionary["BKA"] as string;
                                if (string.IsNullOrWhiteSpace(playerName) && akas != null)
                                {
                                    playerName = akas.First();
                                }
                                if (!string.IsNullOrWhiteSpace(playerName))
                                {
                                    var player = new BucketPlayer(playerName, playerId);
                                    yield return player;
                                }
                            }
                        }
                    }
                }
            }
        } 
    }
}
