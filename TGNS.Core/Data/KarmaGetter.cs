using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IKarmaGetter
    {
        IEnumerable<IKarmaDelta> Get(string realmName, long? playerId);
    }

    public class KarmaGetter : DataAccessor, IKarmaGetter
    {
        public KarmaGetter(string connectionString) : base(connectionString)
        {

        }

        public IEnumerable<IKarmaDelta> Get(string realmName, long? playerId)
        {
            var result = new List<IKarmaDelta>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select KarmaId, KarmaPlayerId, KarmaDeltaName, KarmaDelta, RowCreated from karma where KarmaRealm = @Realm";
                    command.Parameters.AddWithValue("@Realm", realmName);
                    if (playerId.HasValue)
                    {
                        command.CommandText += " and KarmaPlayerId = @PlayerId";
                        command.Parameters.AddWithValue("@PlayerId", playerId.Value);
                    }
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32("KarmaId");
                            var name = reader.GetString("KarmaDeltaName");
                            var amount = reader.GetDecimal("KarmaDelta");
                            var created = reader.GetDateTime("RowCreated");
                            playerId = reader.GetInt64("KarmaPlayerId");
                            result.Add(new KarmaDelta(id, playerId.Value, name, amount, created));
                        }
                    }
                }
            }
            return result;
        }

    }

    public interface IKarmaDelta
    {
        int Id { get; }
        long PlayerId { get; }
        string Name { get; }
        decimal Amount { get; }
        DateTime Created { get; }
    }

    public class KarmaDelta : IKarmaDelta
    {
        public KarmaDelta(int id, long playerId, string name, decimal amount, DateTime created)
        {
            Id = id;
            PlayerId = playerId;
            Name = name;
            Amount = amount;
            Created = created;
        }

        public int Id { get; }
        public long PlayerId { get; }
        public string Name { get; }
        public decimal Amount { get; }
        public DateTime Created { get; }
    }
}
