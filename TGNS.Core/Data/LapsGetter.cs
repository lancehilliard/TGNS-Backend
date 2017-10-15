using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface ILapsGetter
    {
        IEnumerable<ILap> Get(long playerId);
        IEnumerable<ILap> Get();
    }

    public class LapsGetter : DataAccessor, ILapsGetter
    {
        public LapsGetter(string connectionString)
            : base(connectionString)
        {
        }

        IEnumerable<ILap> Get(long? playerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select playerid, trackid, buildnumber, rowcreated, min(seconds) as Seconds, classname from laps";
                    if (playerId.HasValue)
                    {
                        command.CommandText += " WHERE PlayerId = @PlayerId";
                        command.Parameters.AddWithValue("@PlayerId", playerId.Value);
                    }
                    command.CommandText += " group by playerid, trackid, buildnumber, classname;";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var lapPlayerId = reader.GetInt64("PlayerId");
                            var trackId = reader.GetString("TrackId");
                            var rowCreated = reader.GetDateTime("RowCreated");
                            var buildNumber = reader.GetString("BuildNumber");
                            var seconds = reader.GetDecimal("Seconds");
                            var className = reader.GetString("ClassName");
                            yield return new Lap(lapPlayerId, trackId, rowCreated, buildNumber, seconds, className);
                        }
                    }
                }
            }
        }

        public IEnumerable<ILap> Get(long playerId)
        {
            var result = Get(new long?(playerId));
            return result;
        }

        public IEnumerable<ILap> Get()
        {
            var result = Get(null);
            return result;
        } 

    }
}