using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Portal.Classes
{
    public interface ITaglineSetter
    {
        void Set(long playerId, string message);
    }

    public class TaglineSetter : ITaglineSetter
    {
        public void Set(long playerId, string message)
        {
            var taglinesData = new Dictionary<string, object>();
            taglinesData["steamId"] = playerId;
            taglinesData["message"] = string.IsNullOrWhiteSpace(message) ? string.Empty : message;
            var taglinesJson = JsonConvert.SerializeObject(taglinesData);
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    // command.CommandText = "UPDATE data SET datavalue = @DataValue WHERE datarealm = 'ns2' AND datatypename = 'taglines' AND datarecordid = @PlayerId;";
                    command.CommandText = "INSERT INTO data (DataRealm, DataTypeName, DataRecordId, DataValue) VALUES ('ns2', 'taglines', @PlayerId, @DataValue) ON DUPLICATE KEY UPDATE DataValue=@DataValue;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@DataValue", taglinesJson);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}