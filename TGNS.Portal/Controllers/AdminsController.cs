using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Core.Messaging;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class AdminsController : AdminController
    {
        private readonly ISteamBotMessageSender _steamBotMessageSender;
        private readonly IMessagePusher _messagePusher;
        private readonly IMessagePushLogger _messagePushLogger;

        public AdminsController()
        {
            _steamBotMessageSender = new SteamBotMessageSender(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _messagePusher = new PushbulletMessagePusher(ConfigurationManager.AppSettings["PushbulletEncodedAuthKey"]);
            _messagePushLogger = new MessagePushLogger(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        public ActionResult Index()
        {
            var serverModels = _serverGetter.Get();
            var tauntWebAdminBaseUrl = serverModels.First().WebAdminBaseUrl;
            ViewBag.TauntWebAdminUrl = $"{tauntWebAdminBaseUrl}index.html";
            ViewBag.TauntAuthenticatedWebAdminUrl = ViewBag.TauntWebAdminUrl.Replace("http://", "http://tgns:PASSWORD_HERE@");
            return View();
        }

        public ActionResult Broadcast(string title, string message)
        {
            ActionResult result;
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Message = TempData["Message"] ?? "Specify title and message.";
                result = View();
            }
            else
            {
                var pushSummary = _messagePusher.Push("tgns-broadcasts", title, message);
                _messagePushLogger.Log("ns2", PlayerId, _messagePusher.PlatformName, pushSummary.Input, pushSummary.Output, pushSummary.ResultCode, pushSummary.ResultDescription);
                _steamBotMessageSender.Send("ns2", PlayerId, "tgns-broadcasts", title, message, null, null);
                TempData["Message"] = $"Success. ({title} - {message})";
                result = RedirectToAction("Broadcast");
            }
            return result;
        }
	}
}