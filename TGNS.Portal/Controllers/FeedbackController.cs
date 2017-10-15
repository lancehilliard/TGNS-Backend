using System;
using System.Configuration;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Core.Messaging;

namespace TGNS.Portal.Controllers
{
    public class FeedbackController : AuthenticatedController
    {
        private readonly IFeedbacksSetter _feedbacksSetter;
        private readonly IMessagePusher _messagePusher;

        public FeedbackController()
        {
            _feedbacksSetter = new FeedbacksSetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _messagePusher = new PushbulletMessagePusher(ConfigurationManager.AppSettings["PushbulletEncodedAuthKey"]);
        }

        [HttpGet]
        public ActionResult Index(string i, string n, string s)
        {
            if (!string.IsNullOrWhiteSpace(i) && !string.IsNullOrWhiteSpace(n) && !string.IsNullOrWhiteSpace(s))
            {
                var now = DateTime.Now;
                TempData["Subject"] = string.Format("Regarding: {0} ({1})", n, i);
                TempData["Body"] = string.Format("When: {0} Central on {1} (Server: {2})\n\nProvide below any positive or negative feedback about {3}:\n\n", now.ToShortTimeString(), now.ToShortDateString(), s, n);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add(string subject, string body)
        {
            if (!(string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body)))
            {
                _feedbacksSetter.Set(PlayerId, subject, body);
                _messagePusher.Push("tgns-admin", "Feedback", "Feedback has been shared.");
                TempData["Success"] = "Feedback submitted successfully.";
            }
            else
            {
                TempData["Subject"] = subject;
                TempData["Body"] = body;
                TempData["Error"] = "Both Subject and Body are required.";
            }
            return RedirectToAction("Index", "Feedback");
        }
	}
}