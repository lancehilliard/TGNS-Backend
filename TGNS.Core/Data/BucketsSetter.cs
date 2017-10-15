using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBucketsSetter
    {
        void Set(IBuckets buckets);
    }

    public class BucketsSetter : DataAccessor, IBucketsSetter
    {
        public BucketsSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(IBuckets buckets)
        {
            var commDictionaries = buckets.CommPlayers.Select(x => new Dictionary<string, object> { { "id", x.Id }, { "name", x.Name } });
            var bestPlayerDictionaries = buckets.BestPlayers.Select(x => new Dictionary<string, object> { { "id", x.Id }, { "name", x.Name } });
            var betterPlayerDictionaries = buckets.BetterPlayers.Select(x => new Dictionary<string, object> { { "id", x.Id }, { "name", x.Name } });
            var goodPlayerDictionaries = buckets.GoodPlayers.Select(x => new Dictionary<string, object> { { "id", x.Id }, { "name", x.Name } });
            var bucketsDictionary = new Dictionary<string, IEnumerable<Dictionary<string, object>>>
            {
                {"Commanders", commDictionaries},
                {"BestPlayers", bestPlayerDictionaries},
                {"BetterPlayers", betterPlayerDictionaries},
                {"GoodPlayers", goodPlayerDictionaries}
            };
            var bucketsJson = JsonConvert.SerializeObject(bucketsDictionary);
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"update data set datavalue=@BucketsJson where datatypename = 'notedplayers' and datarealm = 'ns2';";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BucketsJson", bucketsJson);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}