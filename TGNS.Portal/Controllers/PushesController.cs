using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Core.Messaging;

namespace TGNS.Portal.Controllers
{
    public class PushesController : AdminController
    {
        private readonly IPushLogsGetter _pushLogsGetter;
        private readonly IMessagePusher _messagePusher;
        private readonly IMessagePushLogger _messagePushLogger;

        public PushesController()
        {
            _pushLogsGetter = new PushLogsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _messagePusher = new PushbulletMessagePusher(ConfigurationManager.AppSettings["PushbulletEncodedAuthKey"]);
            _messagePushLogger = new MessagePushLogger(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var pushLogs = _pushLogsGetter.Get("ns2").Take(100);
            return View(pushLogs);
        }

        [HttpPost]
        public ActionResult Send(string channelId, string title, string message)
        {

            if (string.IsNullOrWhiteSpace(channelId) || string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            {
                TempData["error"] = "Channel, Title, and Message are required when sending notifications.";
                TempData["PushInputChannelId"] = channelId;
                TempData["PushInputTitle"] = title;
                TempData["PushInputMessage"] = message;
            }
            else
            {
                var pushSummary = _messagePusher.Push(channelId, title, message);
                _messagePushLogger.Log("ns2", PlayerId, _messagePusher.PlatformName, pushSummary.Input, pushSummary.Output, pushSummary.ResultCode, pushSummary.ResultDescription);
                if (_messagePusher.WasSuccessful(pushSummary))
                {
                    TempData["success"] = "Push successful.";
                }
                else
                {
                    TempData["error"] = "Push failed. See log below for details.";
                }
            }
            return RedirectToAction("Index", "Pushes");
        }
    }
}