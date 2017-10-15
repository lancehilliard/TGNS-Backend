using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SteamKit2;
using TGNS.Core.Data;

namespace TGNS.Portal.Classes
{
    public interface ISteamBotMessageSender
    {
        void Send(string realmName, long sourcePlayerId, string offeringName, string title, string message, IReadOnlyCollection<EPersonaState> messageHaltingPersonaStates, IReadOnlyCollection<long> playerIdsToIgnore);
    }

    public class SteamBotMessageSender : ISteamBotMessageSender
    {
        private readonly INotificationSubscribersGetter _notificationSubscribersGetter;
        private readonly IPlayerIdAdapter _playerIdAdapter;
        private readonly INotificationsLogLineAdder _notificationsLogLineAdder;

        public SteamBotMessageSender(string connectionString)
        {
            _notificationSubscribersGetter = new NotificationSubscribersGetter(connectionString);
            _playerIdAdapter = new PlayerIdAdapter();
            _notificationsLogLineAdder = new NotificationsLogLineAdder(connectionString);
        }

        public void Send(string realmName, long sourcePlayerId, string offeringName, string title, string message, IReadOnlyCollection<EPersonaState> messageHaltingPersonaStates, IReadOnlyCollection<long> playerIdsToIgnore)
        {
            var subscribedPlayerIds = _notificationSubscribersGetter.Get(realmName, offeringName);
            var friendPlayerIds = ScheduledTaskRegistry.BotTask.B.FriendsList.Select(x => Convert.ToInt64((uint) x.AccountID));
            var subscribedFriendPlayerIds = subscribedPlayerIds.Intersect(friendPlayerIds);
            var subscribedFriendSteamIds = _playerIdAdapter.Adapt((IEnumerable<long>) subscribedFriendPlayerIds);
            var logSentWhen = DateTime.Now;
            foreach (var subscribedFriendSteamId in subscribedFriendSteamIds)
            {
                if (playerIdsToIgnore == null || !playerIdsToIgnore.Contains(Convert.ToInt64(subscribedFriendSteamId.AccountID)))
                {
                    var friendPersonaState = ScheduledTaskRegistry.BotTask.B.SteamFriends.GetFriendPersonaState(subscribedFriendSteamId);
                    if (messageHaltingPersonaStates == null || !messageHaltingPersonaStates.Contains(friendPersonaState))
                    {
                        ScheduledTaskRegistry.BotTask.B.SteamFriends.SendChatMessage(subscribedFriendSteamId, EChatEntryType.ChatMsg, $"{title} - {message}");
                        _notificationsLogLineAdder.Add(realmName, logSentWhen, sourcePlayerId, Convert.ToInt64(subscribedFriendSteamId.AccountID), offeringName, title, message);
                    }
                }
            }
        }
    }
}