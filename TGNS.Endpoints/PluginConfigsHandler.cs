using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints
{
    public class PluginConfigsHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            return true;
        }

        protected override void Write(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        // http://localhost:4155/pluginConfigs/v1?p=PASSWORD_HERE&r=ns2dev&plugins=[%22ban%22,%22scoreboard%22]&gamemode=ns2
        protected override string GetResponseJson(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            var pluginDatas = new Dictionary<string, Dictionary<string, object>>();
            var pluginNames = JsonConvert.DeserializeObject<IList<string>>(request["plugins"]);
            var requestedPluginGameMode = request["gamemode"];
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "SELECT PluginName, PluginConfig FROM plugins WHERE PluginRealm=@PluginRealm AND PluginGameMode=@PluginGameMode;";
                command.Prepare();
                command.Parameters.AddWithValue("@PluginRealm", realmName);
                command.Parameters.AddWithValue("@PluginGameMode", requestedPluginGameMode);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pluginName = reader.GetString("PluginName");
                        if (pluginNames.Contains(pluginName))
                        {
                            var pluginConfig = reader.GetString("PluginConfig");
                            var pluginSuccess = false;
                            var pluginData = new Dictionary<string, object>();
                            try
                            {
                                JsonConvert.DeserializeObject(pluginConfig);
                                pluginSuccess = true;
                                pluginData.Add("config", new JRaw(pluginConfig));
                                pluginData.Add("gamemode", requestedPluginGameMode);
                            }
                            catch (Exception exception)
                            {
                                pluginData.Add("msg", exception.Message);
                                pluginData.Add("stacktrace", exception.StackTrace.Replace(Environment.NewLine, @" \ "));
                            }
                            pluginData.Add("success", pluginSuccess);
                            pluginDatas.Add(pluginName, pluginData);
                            pluginNames.Remove(pluginName);
                        }
                    }
                }
            }
            foreach (var pluginName in pluginNames)
            {
                var pluginData = new Dictionary<string, object>();
                pluginData.Add("success", false);
                pluginData.Add("msg", "No config stored.");
                pluginDatas.Add(pluginName, pluginData);
            }
            var response = new Dictionary<string, object>();
            response.Add("success", true);
            response.Add("plugins", pluginDatas);
            var result = JsonConvert.SerializeObject(response);
            return result;
        }
    }
}