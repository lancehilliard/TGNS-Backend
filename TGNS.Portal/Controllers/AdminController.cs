using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class AdminController : AuthenticatedController
    {
        private readonly IPlayerAdminChecker _playerAdminChecker;
        public AdminController()
        {
            _playerAdminChecker = new PlayerAdminChecker();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_playerAdminChecker.IsAdmin(PlayerId))
            {
                throw new SecurityException("Current user is not a recognized NS2 admin.");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}