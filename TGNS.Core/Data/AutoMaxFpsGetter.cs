using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IAutoMaxFpsGetter
    {
        byte? Get(long playerId);
    }

    public class AutoMaxFpsGetter : DataAccessor, IAutoMaxFpsGetter
    {
        public AutoMaxFpsGetter(string connectionString) : base(connectionString)
        {
        }

        public byte? Get(long playerId)
        {
            var result = default(byte?);
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select maxfps from automaxfps where playerid = @PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.CommandTimeout = 120;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetByte("maxfps");
                        }
                    }
                }
            }
            return result;
        }
    }
}