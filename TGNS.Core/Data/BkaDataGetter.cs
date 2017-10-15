using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IBkaDataGetter
    {
        IBkaData Get(long playerId);
        IEnumerable<IBkaData> Get();
    }

    public class BkaDataGetter : DataAccessor, IBkaDataGetter
    {
        private readonly IBkaDataParser _bkaDataParser;

        public IBkaData Get(long playerId)
        {
            IBkaData result = null;
            string bkaJson = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT datavalue FROM data WHERE datatypename = 'bka' and datarealm = 'ns2' AND datarecordid = @PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bkaJson = reader.GetString("datavalue");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(bkaJson))
            {
                result = _bkaDataParser.Parse(bkaJson);
            }
            return result;
        }

        public IEnumerable<IBkaData> Get()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select * from data where datatypename = 'bka' and replace(datavalue, ' ','') not like '%""BKA"":""""%' and datarealm = 'ns2';";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bkaJson = reader.GetString("datavalue");
                            if (!string.IsNullOrWhiteSpace(bkaJson))
                            {
                                yield return _bkaDataParser.Parse(bkaJson);
                            }
                        }
                    }
                }
            }
        }

        public BkaDataGetter(string connectionString, IBkaDataParser bkaDataParser) : base(connectionString)
        {
            _bkaDataParser = bkaDataParser;
        }
    }
}