using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IAutoMaxFpsSetter
    {
        void Set(long playerId, int maxFps);
    }

    public class AutoMaxFpsSetter : DataAccessor, IAutoMaxFpsSetter
    {
        public AutoMaxFpsSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(long playerId, int maxFps)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO automaxfps (PlayerId, MaxFps) VALUES (@PlayerId, @MaxFps) ON DUPLICATE KEY UPDATE MaxFps=@MaxFps;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.Parameters.AddWithValue("@MaxFps", maxFps);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}