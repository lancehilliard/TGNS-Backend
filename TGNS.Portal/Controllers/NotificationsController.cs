using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using SteamKit2;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class NotificationsController : AuthenticatedController
    {
        private readonly INotificationSubscriptionsGetter _notificationSubscriptionsGetter;
        private readonly INotificationSubscriptionSetter _notificationSubscriptionSetter;
        private readonly IPlayerIdAdapter _playerIdAdapter;
        

        public NotificationsController()
        {
            _notificationSubscriptionsGetter = new NotificationSubscriptionsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _notificationSubscriptionSetter = new NotificationSubscriptionSetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _playerIdAdapter = new PlayerIdAdapter();
        }

        public ActionResult Index()
        {
            var notificationSubscriptions = _notificationSubscriptionsGetter.Get("ns2", PlayerId).Where(x=>PlayerId==160301 || !string.Equals(x.OfferingName, "tgns-test", StringComparison.InvariantCultureIgnoreCase));
            var playerIsBotFriend = ScheduledTaskRegistry.BotTask.B != null && ScheduledTaskRegistry.BotTask.B.FriendsList.Any(x=>Convert.ToInt64(x.AccountID).Equals(PlayerId));
            return View(new NotificationsIndexViewModel {Subscriptions=notificationSubscriptions, IsBotFriend= playerIsBotFriend });
        }

        public JsonResult Set(string offeringName, string offeringDisplayName, bool subscribed)
        {
            var response = new Dictionary<string, object>();
            try
            {
                _notificationSubscriptionSetter.Set("ns2", PlayerId, offeringName, subscribed);
                var friendRelationship = ScheduledTaskRegistry.BotTask.B.SteamFriends.GetFriendRelationship(PlayerSteamID);
                if (friendRelationship == EFriendRelationship.Friend)
                {
                    var statusChange = subscribed ? "subscribed to" : "unsubscribed from";
                    var message = $"Successfully {statusChange} '{offeringDisplayName}' notification. Manage notifications: http://rr.tacticalgamer.com/Notifications";
                    ScheduledTaskRegistry.BotTask.B.SteamFriends.SendChatMessage(PlayerSteamID, EChatEntryType.ChatMsg, message);
                }
                else if (subscribed)
                {
                    ScheduledTaskRegistry.BotTask.B.SteamFriends.AddFriend(PlayerSteamID);
                }
                response["success"] = true;
            }
            catch (Exception e)
            {
                response["msg"] = e.Message;
            }
            return Json(JsonConvert.SerializeObject(response));
        }
    }

    public class NotificationsIndexViewModel
    {
        public IEnumerable<INotificationSubscription> Subscriptions { get; set; }
        public bool IsBotFriend { get; set; }
    }
}