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
    public interface IBucketsGetter
    {
        IBuckets Get();
    }

    public class BucketsGetter : DataAccessor, IBucketsGetter
    {
        public BucketsGetter(string connectionString) : base(connectionString)
        {
        }

        private IEnumerable<IBucketPlayer> GetPlayers(Dictionary<string, IEnumerable<Dictionary<string, object>>> bucketsData, string bucketName)
        {
            var bucketData = bucketsData[bucketName];
            return (from playerData in bucketData let name = playerData["name"] as string let id = Convert.ToInt64(playerData["id"]) select new BucketPlayer(name, id));
        }

        public IBuckets Get()
        {
            string bucketsJson = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select datavalue from data where datatypename = 'notedplayers' and datarealm = 'ns2';";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bucketsJson = reader.GetString("datavalue");
                        }
                    }
                }
            }
            var bucketsData = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<Dictionary<string, object>>>>(bucketsJson);
            var commPlayers = GetPlayers(bucketsData, "Commanders");
            var bestPlayers = GetPlayers(bucketsData, "BestPlayers");
            var betterPlayers = GetPlayers(bucketsData, "BetterPlayers");
            var goodPlayers = GetPlayers(bucketsData, "GoodPlayers");
            var result = new Buckets(commPlayers, bestPlayers, betterPlayers, goodPlayers);
            return result;
        }
    }
}
