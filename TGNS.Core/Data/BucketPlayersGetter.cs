//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MySql.Data.MySqlClient;
//using Newtonsoft.Json;
//using TGNS.Core.Domain;

//namespace TGNS.Core.Data
//{
//    public interface IBucketPlayersGetter
//    {
//        Dictionary<int, IEnumerable<string>> Get(string realmName);
//    }

//    public class BucketPlayersGetter : DataAccessor, IBucketPlayersGetter
//    {
//        public BucketPlayersGetter(string connectionString) : base(connectionString)
//        {
//        }

//        public Dictionary<int, IEnumerable<string>> Get(string realmName)
//        {
//            var result = new Dictionary<int, IEnumerable<string>>(); 
//            using (var connection = new MySqlConnection(ConnectionString))
//            {
//                connection.Open();
//                using (var command = new MySqlCommand { Connection = connection })
//                {
//                    command.CommandText = "SELECT DataRecordId, DataValue FROM data WHERE DataRealm=@DataRealm and DataTypeName = 'bka'";
//                    command.Prepare();
//                    command.Parameters.AddWithValue("@DataRealm", realmName);
//                    using (var reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            var playerId = reader.GetInt64("DataRecordId");
//                            var bkaJson = reader.GetString("DataValue");
//                            bkaJson = bkaJson.Replace(@"\", @"\\");
//                            var bkaDictionary = JsonConvert.DeserializeObject<Dictionary<string,object>>(bkaJson);
//                            var akas = bkaDictionary["AKAs"] as IEnumerable<string>;
//                            result.Add(playerId, akas);
//                        }
//                    }
//                }
//            }
//            return result;
//        }
//    }
//}
