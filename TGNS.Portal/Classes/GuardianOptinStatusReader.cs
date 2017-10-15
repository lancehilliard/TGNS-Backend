using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Portal.Classes
{
    public interface IGuardianOptinStatusReader
    {
        bool IsOptedIn(long playerId);
    }

    public class GuardianOptinStatusReader : IGuardianOptinStatusReader
    {
        public bool IsOptedIn(long playerId)
        {
            var result = false;
            string guardianOptInJson = null;
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select datavalue from data where datatypename = 'guardian' and datarealm = 'ns2' and datarecordid = @PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            guardianOptInJson = reader.GetString("datavalue");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(guardianOptInJson))
            {
                var guardianOptInData = JsonConvert.DeserializeObject<Dictionary<string, object>>(guardianOptInJson);
                result = (bool) guardianOptInData["optin"];
            }
            return result;
        }
    }
}