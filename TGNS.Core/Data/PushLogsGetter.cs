using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;
using TGNS.Core.Messaging;

namespace TGNS.Core.Data
{
    public interface IPushLogsGetter
    {
        IEnumerable<IPushLog> Get(string realmName);
    }

    public class PushLogsGetter : DataAccessor, IPushLogsGetter
    {
        public PushLogsGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IPushLog> Get(string realmName)
        {
            var result = new List<IPushLog>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT PushPlayerId, PushPlatform, PushInput, PushOutput, PushResultCode, PushResultDescription, RowCreated, RowUpdated FROM pushes WHERE PushRealm=@Realm ORDER BY RowCreated DESC;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@Realm", realmName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("PushPlayerId");
                            var platformName = reader.GetString("PushPlatform");
                            var input = reader.GetString("PushInput");
                            var output = reader.GetString("PushOutput");
                            var resultCode = reader.GetInt32("PushResultCode");
                            var resultDescription = reader.GetString("PushResultDescription");
                            var created = reader.GetDateTime("RowCreated");
                            var lastModified = reader.GetDateTime("RowUpdated");
                            result.Add(new PushLog(realmName, playerId, platformName, new PushSummary { Input = input, Output = output, ResultCode = resultCode, ResultDescription = resultDescription }, created, lastModified));
                        }
                    }
                }
            }
            return result;
        }
    }
}