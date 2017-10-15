using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface INotificationSubscribersGetter
    {
        IEnumerable<long> Get(string realmName, string offeringName);
    }

    public class NotificationSubscribersGetter : DataAccessor, INotificationSubscribersGetter
    {
        public NotificationSubscribersGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<long> Get(string realmName, string offeringName)
        {
            var result = new List<long>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select playerid from notifications_subscriptions s inner join notifications_offerings o on s.OfferingName = o.Name where s.RealmName = @RealmName and o.Name = @Name and s.Subscribed = TRUE";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RealmName", realmName);
                    command.Parameters.AddWithValue("@Name", offeringName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("PlayerId");
                            result.Add(playerId);
                        }
                    }
                }
            }
            return result;
        }
    }
}