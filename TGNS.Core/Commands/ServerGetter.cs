using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Commands
{
    public interface IServerGetter
    {
        IEnumerable<IServerModel> Get();
    }

    public class ServerGetter : IServerGetter
    {
        public IEnumerable<IServerModel> Get()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT 
    ServerId,
    ServerName,
    WebAdminBaseUrl,
    InstanceIndex
FROM servers
ORDER BY ServerId;";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32("ServerId");
                            var serverName = reader.GetString("ServerName");
                            var webAdminBaseUrl = reader.GetString("WebAdminBaseUrl");
                            var instanceIndex = reader.GetInt32("InstanceIndex");
                            var result = new ServerModel(id, serverName, webAdminBaseUrl, instanceIndex);
                            yield return result;
                        }
                    }
                }
            }
        }
    }
}