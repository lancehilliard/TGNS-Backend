using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface ITaglineGetter
    {
        string Get(long playerId);
    }

    public class TaglineGetter : ITaglineGetter
    {
        public string Get(long playerId)
        {
            var result = string.Empty;
            string taglinesJson = null;
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT datavalue FROM data WHERE datatypename = 'taglines' and datarealm = 'ns2' AND datarecordid = @PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            taglinesJson = reader.GetString("datavalue");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(taglinesJson))
            {
                taglinesJson = taglinesJson.Replace(@"""", "\"");
                var taglinesData = JsonConvert.DeserializeObject<Dictionary<string, object>>(taglinesJson);
                var taglineMessage = taglinesData["message"] as string;
                if (!string.IsNullOrWhiteSpace(taglineMessage))
                {
                    result = taglineMessage;
                }
            }
            return result;
        }
    }
}