using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class SpraysHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = !string.IsNullOrWhiteSpace(request["m"]);
            return result;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var mapName = Convert.ToString(request["mapName"]);
            var path = Convert.ToString(request["path"]);
            var playerId = Convert.ToInt64(request["playerid"]);
            var x = Convert.ToDouble(request["x"]);
            var y = Convert.ToDouble(request["y"]);
            var z = Convert.ToDouble(request["z"]);
            var yaw = Convert.ToDouble(request["yaw"]);
            var roll = Convert.ToDouble(request["roll"]);
            var pitch = Convert.ToDouble(request["pitch"]);
            command.CommandText = "INSERT INTO sprays (MapName, Path, PlayerId, OriginX, OriginY, OriginZ, Yaw, Roll, Pitch) VALUES (@MapName, @Path, @PlayerId, @X, @Y, @Z, @Yaw, @Roll, @Pitch) ON DUPLICATE KEY UPDATE OriginX=@X, OriginY=@Y, OriginZ=@Z, Yaw=@Yaw, Roll=@Roll, Pitch=@Pitch, RowUpdated=CURRENT_TIMESTAMP;";
            command.Prepare();
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@MapName", mapName);
            command.Parameters.AddWithValue("@Path", path);
            command.Parameters.AddWithValue("@PlayerId", playerId);
            command.Parameters.AddWithValue("@X", x);
            command.Parameters.AddWithValue("@Y", y);
            command.Parameters.AddWithValue("@Z", z);
            command.Parameters.AddWithValue("@Yaw", yaw);
            command.Parameters.AddWithValue("@Roll", roll);
            command.Parameters.AddWithValue("@Pitch", pitch);
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var mapName = request["m"];
            var sprays = new List<IDictionary<string, object>>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"SELECT 
                                            MapName,
                                            Path,
                                            PlayerId,
                                            OriginX,
                                            OriginY,
                                            OriginZ,
                                            Yaw,
                                            Roll,
                                            Pitch,
                                            RowCreated,
                                            RowUpdated
                                        FROM
                                            sprays s1
                                        WHERE
                                            s1.RowUpdated = (SELECT 
                                                    s2.RowUpdated
                                                FROM
                                                    sprays s2
                                                WHERE
                                                    s2.playerid = s1.playerid
                                                ORDER BY s2.RowUpdated DESC
                                                LIMIT 1)
	                                        AND s1.RowUpdated > DATE_SUB(CURRENT_DATE, INTERVAL 3 MONTH)
	                                        AND s1.MapName = @MapName";
                command.Prepare();
                command.Parameters.AddWithValue("@MapName", mapName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var spray = new Dictionary<string, object>();
                        spray.Add("path", reader.GetString("path"));
                        spray.Add("playerid", reader.GetInt64("PlayerId"));
                        spray.Add("x", reader.GetFloat("OriginX"));
                        spray.Add("y", reader.GetFloat("OriginY"));
                        spray.Add("z", reader.GetFloat("OriginZ"));
                        spray.Add("yaw", reader.GetFloat("Yaw"));
                        spray.Add("roll", reader.GetFloat("Roll"));
                        spray.Add("pitch", reader.GetFloat("Pitch"));
                        sprays.Add(spray);
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "sprays", sprays } });
            return result;
        }
    }
}