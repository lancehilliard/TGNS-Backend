using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SteamKit2;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Core.Steam;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class EndpointsController : PasswordedEndpointsController
    {
        public static readonly IDictionary<long, IList<string>> ContactQueries = new Dictionary<long, IList<string>>();
        private readonly IServerCurrentInfoDictionaryGetter _serverCurrentInfoDictionaryGetter;
        private readonly IServerGetter _serverGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;
        private readonly ISteamBotMessageSender _steamBotMessageSender;
        private static readonly List<EPersonaState> EPersonaStatesThatHaltNotifications = new List<EPersonaState> { EPersonaState.Offline, EPersonaState.Busy };


        public EndpointsController()
        {
            _serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _serverGetter = new ServerGetter();
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
            _steamBotMessageSender = new SteamBotMessageSender(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        JsonResult Handle(Func<object> resultGetter)
        {
            var response = new Dictionary<string, object>();
            try
            {
                response["result"] = resultGetter();
                response["success"] = true;
            }
            catch (Exception e)
            {
                response["msg"] = e.Message;
                response["stacktrace"] = e.StackTrace.Replace(Environment.NewLine, @" \ ");
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Notify(string realmName, long sourcePlayerId, string offeringName, string title, string message)
        {
            //return Handle(() =>
            //{
            //    var serverPlayers = new List<IServerPlayer>();
            //    var serverModels = _serverGetter.Get();
            //    foreach (var serverModel in serverModels)
            //    {
            //        var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
            //        var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
            //        serverPlayers.AddRange(serverCurrentInfo.Players);
            //    }
            //    _steamBotMessageSender.Send(realmName, sourcePlayerId, offeringName, title, message, EPersonaStatesThatHaltNotifications, serverPlayers.Select(x=>x.PlayerId).ToList().AsReadOnly());
            //    return null;
            //});
            return null;
        }

        public JsonResult QueryContact(long sourcePlayerId, long targetPlayerId, string targetPlayerName)
        {
            return Handle(() =>
            {
                var queries = new List<string>();
                if (ContactQueries.ContainsKey(sourcePlayerId))
                {
                    queries.AddRange(ContactQueries[sourcePlayerId]);
                }
                var steamIdConverter = new SteamIdConverter();
                var steamCommunityProfileId = steamIdConverter.GetSteamCommunityProfileIdFromNs2Id(targetPlayerId);
                var url = $"http://steamcommunity.com/profiles/{steamCommunityProfileId}";
                var entry = string.Format($@"<a href=""{url}"" target=""_blank"">{HttpUtility.HtmlEncode(targetPlayerName)}</a>");
                if (!queries.Contains(entry))
                {
                    queries.Add(entry);
                }
                ContactQueries[sourcePlayerId] = queries.Skip(Math.Max(0, queries.Count - 10)).ToList();

                return null;
            });
        }
    }
}