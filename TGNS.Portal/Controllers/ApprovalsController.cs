using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class ApprovalsController : AdminController
    {
        private readonly IApprovalsGetter _approvalsGetter;
        private readonly IApprovalsRemover _approvalsRemover;
        public ApprovalsController()
        {
            _approvalsGetter = new ApprovalsGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _approvalsRemover = new ApprovalsRemover(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var approvals = _approvalsGetter.Get("ns2").Where(x=>x.Created >= DateTime.Now.AddMonths(-1)).OrderByDescending(x => x.Created);
            var viewModel = new ApprovalsViewModel(approvals);
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(string realm, string serverName, double startTimeSeconds, long sourcePlayerId, long targetPlayerId)
        {
            _approvalsRemover.Remove(realm, serverName, startTimeSeconds, sourcePlayerId, targetPlayerId);
            TempData["Success"] = string.Format("Approval removed (Source: {0}; Target: {1}).", sourcePlayerId, targetPlayerId);
            return RedirectToAction("Manage");
        }
	}
}