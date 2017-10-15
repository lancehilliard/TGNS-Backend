using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints.Badges
{
    public class PlayerPropertiesHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["v"]);
            return result;
        }

        protected override void Write(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlCommand command)
        {
            var playerId = request["i"];
            var propertyName = request["n"];
            var propertyValue = Convert.ToInt64(request["v"]);
            var shouldMarkPropertyAsActive = string.Equals("true", request["a"], StringComparison.InvariantCultureIgnoreCase);
            command.CommandText = "INSERT INTO achievements_properties_players (AchievementsRealm, PropertyName, PlayerId, PropertyValue) VALUES (@AchievementsRealm, @PropertyName, @PlayerId, @PropertyValue) ON DUPLICATE KEY UPDATE PropertyValue=@PropertyValue;";
            if (shouldMarkPropertyAsActive)
            {
                command.CommandText += "UPDATE achievements_properties_players SET PropertyIsActive = true WHERE AchievementsRealm = @AchievementsRealm AND PropertyName = @PropertyName AND PlayerId = @PlayerId;";
            }
            command.Prepare();
            command.Parameters.AddWithValue("@AchievementsRealm", realmName);
            command.Parameters.AddWithValue("@PropertyName", propertyName);
            command.Parameters.AddWithValue("@PlayerId", playerId);
            command.Parameters.AddWithValue("@PropertyValue", propertyValue);
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            var playerId = request["i"];
            var excludeActiveProperties = !string.Equals("true", request["a"], StringComparison.InvariantCultureIgnoreCase);
            var properties = new List<Dictionary<string, object>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                //command.CommandText = "SELECT prop.PropertyName, prop.PropertyDescription, prop.PropertyUpdateConstraint, prop.PropertyActivationRule, prop.PropertyActivationValue, plyr.PropertyValue, CONVERT(IFNULL(plyr.PropertyIsActive, 0) using latin1) as PropertyIsActive FROM achievements_properties prop LEFT JOIN achievements_properties_players plyr on prop.AchievementsRealm = plyr.AchievementsRealm and prop.PropertyName = plyr.PropertyName WHERE prop.AchievementsRealm = @AchievementsRealm AND (plyr.PlayerId IS NULL OR plyr.PlayerId = @PlayerId)";
                command.CommandText = "SELECT prop.PropertyName, prop.PropertyDescription, prop.PropertyUpdateConstraint, prop.PropertyActivationRule, prop.PropertyActivationValue, plyr.PropertyValue, CONVERT(IFNULL(plyr.PropertyIsActive, 0) using latin1) as PropertyIsActive FROM achievements_properties prop LEFT JOIN achievements_properties_players plyr on prop.AchievementsRealm = plyr.AchievementsRealm and prop.PropertyName = plyr.PropertyName AND plyr.PlayerId = @PlayerId WHERE prop.AchievementsRealm = @AchievementsRealm";
                if (excludeActiveProperties)
                {
                    command.CommandText += " AND IFNULL(plyr.PropertyIsActive, false) <> true";
                }
                command.Prepare();
                command.Parameters.AddWithValue("@AchievementsRealm", realmName);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var property = new Dictionary<string, object>();
                        property.Add("Name", reader.GetString("PropertyName"));
                        property.Add("Description", reader.GetString("PropertyDescription"));
                        property.Add("UpdateConstraint", reader.GetChar("PropertyUpdateConstraint"));
                        property.Add("ActivationRule", reader.GetChar("PropertyActivationRule"));
                        property.Add("ActivationValue", reader.GetInt64("PropertyActivationValue"));
                        property.Add("Value", reader.IsDBNull(reader.GetOrdinal("PropertyValue")) ? null : (object)reader.GetInt64("PropertyValue"));
                        property.Add("IsActive", reader.GetInt16("PropertyIsActive") == 1);
                        properties.Add(property);
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", properties } });
            return result;
        }
    }
}