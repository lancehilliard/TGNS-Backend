using System;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;
using TGNS.Core.Messaging;

namespace TGNS.Endpoints
{
    public class PushbulletPushHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            return false;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            //var pushChannel = request["c"];
            //var pushTitle = request["t"];
            //var pushMessage = request["m"];
            //var playerId = Convert.ToInt64(request["i"]);
            //var pushbulletMessagePusher = new PushbulletMessagePusher(ConfigurationManager.AppSettings["PushbulletEncodedAuthKey"]);
            //var messagePushLogger = new MessagePushLogger(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            //var pushSummary = pushbulletMessagePusher.Push(pushChannel, pushTitle, pushMessage);
            //messagePushLogger.Log(realmName, playerId, pushbulletMessagePusher.PlatformName, pushSummary.Input, pushSummary.Output, pushSummary.ResultCode, pushSummary.ResultDescription);
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}