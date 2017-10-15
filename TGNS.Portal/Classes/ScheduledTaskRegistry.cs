using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Hosting;
using FluentScheduler;
using MySql.Data.MySqlClient;
using SteamBot;
using SteamKit2;
using TGNS.Bots.Steam;
using TGNS.Core.Data;
using TGNS.Core.Steam;
using Configuration = SteamBot.Configuration;

namespace TGNS.Portal.Classes
{
    public class ScheduledTaskRegistry : Registry
    {
        private readonly Action _sendDailyMessages = () =>
        {
            BotTask.SendDailySteamMessages();
        };

        private readonly Action _updateReplayYouTubeStatistics = () =>
        {
            try
            {
                var replayVideoStatisticsGetter = new ReplayVideoStatisticsGetter();
                var gameRecordingsGetter = new GameRecordingsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                var gameRecordingProviderStatisticsSetter = new GameRecordingProviderStatisticsSetter();
                var recordedGames = gameRecordingsGetter.Get();
                var videoIds = recordedGames.Select(x => x.VideoIdentifier).ToList();
                var statistics = replayVideoStatisticsGetter.GetStatistics(videoIds).ToList();
                foreach (var stat in statistics)
                {
                    gameRecordingProviderStatisticsSetter.Set(stat);
                }
            }
            catch (Exception e)
            {
                BotTask.AddDailySteamMessage(new DailySteamMessage(DateTime.Now, new SteamID("STEAM_0:1:80150"), $"Error updating Replay YouTube stats. Message: {e.Message}; StackTrace: {e.StackTrace}"));
            }
        };

        private readonly Action _updateServerAdminJson = () => {
            var serverAdminJsonUpdater = new ServerAdminJsonUpdater();
            serverAdminJsonUpdater.Update();
        };

        private readonly Action _sendFriendRequests = () =>
        {
            var sendErrors = new List<string>();
            try
            {
                var steamIdConverter = new SteamIdConverter();
                var friendRequestsLogger = new FriendRequestsLogger(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                var chatBotsLogLineAdder = new ChatBotsLogLineAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                var friendRequestsGetter = new FriendRequestsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                var targetPlayerIds = friendRequestsGetter.GetFriendRequestTargetNs2Ids();
                var existingFriends = BotTask.B.FriendsList.ToList();
                var steamIds = targetPlayerIds.Select(x => new SteamID(steamIdConverter.GetReadableSteamIdFromNs2Id(x)));
                var friendRequestTargets = steamIds.Where(x => !existingFriends.Contains(x));
                foreach (var friendRequestTarget in friendRequestTargets)
                {
                    try
                    {
                        BotTask.B.AddFriend(friendRequestTarget);
                        friendRequestsLogger.Log("Bot", Convert.ToInt64(friendRequestTarget.AccountID));
                    }
                    catch (Exception e)
                    {
                        chatBotsLogLineAdder.Add("Bot", "ERROR", $"Error sending friend request to {friendRequestTarget.AccountID}: {e.Message}");
                        sendErrors.Add(friendRequestTarget.AccountID.ToString());
                    }
                }
                if (sendErrors.Any())
                {
                    BotTask.AddDailySteamMessage(new DailySteamMessage(DateTime.Now, new SteamID("STEAM_0:1:80150"), $"Error sending bot friend requests: {string.Join(", ", sendErrors)}"));
                }
            }
            catch (Exception e)
            {
                BotTask.AddDailySteamMessage(new DailySteamMessage(DateTime.Now, new SteamID("STEAM_0:1:80150"), $"Error sending bot friend requests. Message: {e.Message}; StackTrace: {e.StackTrace}"));
            }
        };

        private readonly Action _giveBotFriendsKarma = () =>
        {
            try
            {
                var karmaAdder = new KarmaAdder(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                var targetPlayerIds = BotTask.B.FriendsList.Select(x => Convert.ToInt64(x.AccountID));
                foreach (var targetPlayerId in targetPlayerIds)
                {
                    karmaAdder.Add("ns2", targetPlayerId, "SteamBotFriend", 10);
                }
            }
            catch (Exception e)
            {
                BotTask.AddDailySteamMessage(new DailySteamMessage(DateTime.Now, new SteamID("STEAM_0:1:80150"), $"Error giving bot friends Karma. Message: {e.Message}; StackTrace: {e.StackTrace}"));
            }
        };

        /// <summary>
        /// A method to return an instance of the <c>bot.BotControlClass</c>.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <param name="sid">The steamId.</param>
        /// <returns>A <see cref="UserHandler"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown if the control class type does not exist.</exception>
        public static UserHandler UserHandlerCreator(Bot bot, SteamID sid)
        {
            Type controlClass = Type.GetType(bot.BotControlClass);

            if (controlClass == null)
                throw new ArgumentException("Configured control class type was null. You probably named it wrong in your configuration file.", nameof(bot));

            return (UserHandler)Activator.CreateInstance(
                    controlClass, new object[] { bot, sid });
        }

        public ScheduledTaskRegistry() {
            if (ConfigurationManager.AppSettings["IsDevEnvironment"] != "true")
            {
                Schedule(_giveBotFriendsKarma).ToRunEvery(1).Days().At(3, 0);
                Schedule(_sendFriendRequests).ToRunEvery(1).Days().At(4, 0);
                Schedule(_updateReplayYouTubeStatistics).ToRunEvery(1).Days().At(5, 0);
                Schedule(_sendDailyMessages).ToRunEvery(1).Days().At(6, 0);
                Schedule(_updateServerAdminJson).ToRunNow();
                Schedule(_updateServerAdminJson).ToRunEvery(10).Minutes();
                Schedule<BotTask>().ToRunNow();
            }
            else
            {
                // Schedule(_updateServerAdminJson).ToRunNow();
                // Schedule<BotTask>().ToRunNow();
            }
        }

        public class DailySteamMessage
        {

            public DailySteamMessage(DateTime when, SteamID steamId, string message)
            {
                When = when;
                SteamId = steamId;
                Message = message;
            }

            public DateTime When { get; }
            public string Message { get; }
            public SteamID SteamId { get; }
        }

        public class BotTask : ITask, IRegisteredObject
        {
            // https://github.com/fluentscheduler/FluentScheduler#using-it-with-aspnet
            public static Bot B;

            static readonly IList<DailySteamMessage> DailySteamMessages = new List<DailySteamMessage>();

            public static void AddDailySteamMessage(DailySteamMessage dailySteamMessage)
            {
                DailySteamMessages.Add(dailySteamMessage);
            }

            public static void SendDailySteamMessages()
            {
                var steamIds = DailySteamMessages.Select(x=>x.SteamId).Distinct();
                foreach (var steamId in steamIds)
                {
                    
                    var messages = DailySteamMessages.Where(x => x.SteamId.Equals(steamId)).OrderBy(x => x.When).Select(x => x.Message).ToList();
                    messages.Insert(0, "Some messages were recently queued for you:");
                    foreach (var message in messages)
                    {
                        B.SteamFriends.SendChatMessage(steamId, EChatEntryType.ChatMsg, message);
                    }
                }
                DailySteamMessages.Clear();
            }

            private readonly object _lock = new object();
            private bool _shuttingDown;

            public BotTask()
            {
                HostingEnvironment.RegisterObject(this);
            }

            public void Execute()
            {
                lock (_lock)
                {
                    if (!_shuttingDown)
                    {
                        B = new Bot(new Configuration.BotInfo
                        {
                            Admins = new List<SteamKit2.SteamID> { new SteamKit2.SteamID("76561197960426029") },
                            ApiKey = "STEAM_API_KEY_HERE",
                            AutoStart = true,
                            BotControlClass = "TGNS.Bots.Steam.SimpleUserHandler, TGNS.Bots.Steam",
                            ChatResponse = "Hello.",
                            ConsoleLogLevel = "Success",
                            DisplayName = string.Equals(ConfigurationManager.AppSettings["SteamBotUsername"], "tgns_dev") ? "Dev Bot" : "Bot",
                            DisplayNamePrefix = "[TGNS] ",
                            FileLogLevel = "Info",
                            LogFile = "Bot.log",
                            MaximumActionGap = 30,
                            SchemaLang = "en",
                            Username = ConfigurationManager.AppSettings["SteamBotUsername"],
                            Password = ConfigurationManager.AppSettings["SteamBotPassword"]
                        },
                                    string.Empty,
                                    UserHandlerCreator,
                                    true);
                        B.StartBot();
                    }
                }
            }

            public void Stop(bool immediate)
            {
                lock (_lock)
                {
                    _shuttingDown = true;
                }
                B?.StopBot();
                HostingEnvironment.UnregisterObject(this);
            }
        }
    }
}