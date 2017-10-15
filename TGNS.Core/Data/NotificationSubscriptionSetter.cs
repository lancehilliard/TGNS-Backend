using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface INotificationSubscriptionSetter
    {
        void Set(string realmName, long playerId, string offeringName, bool subscribed);
    }

    public class NotificationSubscriptionSetter : DataAccessor, INotificationSubscriptionSetter
    {
        public NotificationSubscriptionSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(string realmName, long playerId, string offeringName, bool subscribed)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"insert into notifications_subscriptions(RealmName, PlayerId, OfferingName, Subscribed) VALUES(@RealmName, @PlayerId, @OfferingName, @Subscribed) ON DUPLICATE KEY UPDATE Subscribed=@Subscribed;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RealmName", realmName);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    command.Parameters.AddWithValue("@OfferingName", offeringName);
                    command.Parameters.AddWithValue("@Subscribed", subscribed);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}