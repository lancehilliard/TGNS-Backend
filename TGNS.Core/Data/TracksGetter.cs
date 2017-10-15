using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface ITracksGetter
    {
        IEnumerable<ITrack> Get();
    }

    public class TracksGetter : DataAccessor, ITracksGetter
    {
        public TracksGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<ITrack> Get()
        {
            string tracksJson = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT PluginConfig FROM plugins WHERE PluginName = 'lapstracker' AND PluginRealm='ns2';";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tracksJson = reader.GetString("PluginConfig");
                        }
                    }
                }
            }
            var lapstrackerConfig = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(tracksJson);
            var tracks = lapstrackerConfig["Tracks"];
            foreach (var tracksKey in tracks.Keys)
            {
                var track = tracks[tracksKey];
                var id = tracksKey;
                var name = track["name"] as string;
                var mapName = track["mapName"] as string;
                var locationNames = ((Newtonsoft.Json.Linq.JArray)track["locationNames"]).ToObject<IEnumerable<string>>();
                yield return new Track(id, name, mapName, locationNames);
            }
        }
    }
}
