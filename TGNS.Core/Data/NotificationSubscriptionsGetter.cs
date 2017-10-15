using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface INotificationSubscriptionsGetter
    {
        IEnumerable<INotificationSubscription> Get(string realmName, long playerId);
    }

    public class NotificationSubscriptionsGetter : DataAccessor, INotificationSubscriptionsGetter
    {
        public NotificationSubscriptionsGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<INotificationSubscription> Get(string realmName, long playerId)
        {
            var result = new List<INotificationSubscription>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select o.Name as OfferingName, o.Name as OfferingName, o.DisplayName as OfferingDisplayName, o.Description as OfferingDescription, IF(s.Subscribed,TRUE,FALSE) as Subscribed, s.PlayerId from notifications_offerings o left join (select * from notifications_subscriptions where PlayerId = @PlayerId and RealmName = @RealmName) s on o.Name = s.OfferingName ORDER BY o.DisplayOrder;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RealmName", realmName);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var offeringName = reader.GetString("OfferingName");
                            var offeringDisplayName = reader.GetString("OfferingDisplayName");
                            var offeringDescription = reader.GetString("OfferingDescription");
                            var subscribed = reader.GetBoolean("Subscribed");
                            result.Add(new NotificationSubscription(offeringName, offeringDisplayName, offeringDescription, subscribed));
                        }
                    }
                }
            }
            return result;
        }
    }

    public interface INotificationSubscription
    {
        string OfferingName { get; }
        string OfferingDisplayName { get; }
        string OfferingDescription { get; }
        bool Subscribed { get; }
    }

    public class NotificationSubscription : INotificationSubscription
    {
        public NotificationSubscription(string offeringName, string offeringDisplayName, string offeringDescription, bool subscribed)
        {
            OfferingName = offeringName;
            OfferingDisplayName = offeringDisplayName;
            OfferingDescription = offeringDescription;
            Subscribed = subscribed;
        }

        public string OfferingDescription { get; }
        public string OfferingName { get; }
        public string OfferingDisplayName { get; }
        public bool Subscribed { get; }
    }
}