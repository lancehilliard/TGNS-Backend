using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class CommunityController : Controller
    {
        private readonly IApprovalsGetter _approvalsGetter;
        private readonly IEnumerable<string> _approvalReasons;

        public CommunityController()
        {
            _approvalsGetter = new ApprovalsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _approvalReasons = _approvalsGetter.Get("ns2").Where(x => !string.IsNullOrWhiteSpace(x.Reason) && x.Reason.Length > 3).Distinct().OrderByDescending(x => x.Created).Select(x => x.Reason).Take(40);
        }

        //
        // GET: /Community/
        public ActionResult Index()
        {
            ViewBag.ApprovalReasons = _approvalReasons;
            return View();
        }

        public PartialViewResult Approvals()
        {
            var approvalReasons = _approvalReasons.OrderBy(x => Guid.NewGuid()).Take(4);
            return PartialView(approvalReasons);
        }
	}
}