using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class FeedbackViewerController : AdminController
    {
        private readonly IEnumerable<IFeedback> _feedbacks;
        private readonly IFeedbackReadSetter _feedbackReadSetter;

        public FeedbackViewerController()
        {
            _feedbackReadSetter = new FeedbackReadSetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            var feedbacksGetter = new FeedbacksGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _feedbacks = feedbacksGetter.Get().OrderByDescending(x=>x.Created);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_feedbacks);
        }

        [HttpGet]
        public ActionResult Details(long playerId, DateTime created)
        {
            var feedback = _feedbacks.Single(x => x.PlayerId.Equals(playerId) && x.Created.Equals(created));
            _feedbackReadSetter.Set(playerId, created, PlayerId);
            return View(feedback);
        }

        [HttpGet]
        public JsonResult UnreadCount()
        {
            var unreadCount = _feedbacks.Count(x => !x.ReadPlayerIds.Contains(PlayerId));
            var result = Json(unreadCount, JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}
