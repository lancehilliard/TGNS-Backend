using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using SteamKit2;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    [AuthenticatedOrPlayingAuthorize]
    public class AuthenticatedController : Controller
    {
        //private readonly IPlayerIdGetter _playerIdGetter;
        private readonly IBansGetter _bansGetter;
        private readonly IAccessAuditer _accessAuditer;
        protected readonly IServerGetter _serverGetter;
        readonly IServerCurrentInfoDictionaryGetter _serverCurrentInfoDictionaryGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;

        public AuthenticatedController()
        {
            //_playerIdGetter = new PlayerIdGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _bansGetter = new BansGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _accessAuditer = new AccessAuditer(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _serverGetter = new ServerGetter();
            _serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
        }

        [HttpGet]
        public JsonResult IsBanned()
        {
            var bans = _bansGetter.Get("ns2");
            var isBanned = bans.Any(x => x.PlayerId == PlayerId);
            var result = Json(isBanned, JsonRequestBehavior.AllowGet);
            return result;
        }

        protected string UserName
        {
            get { return HttpContext.Items["UserName"] as string; }
        }

        protected long PlayerId
        {
            get
            {
                return (long)HttpContext.Items["PlayerId"];
            }
        }

        protected SteamID PlayerSteamID
        {
            get
            {
                var playerIdAdapter = new PlayerIdAdapter();
                var result = playerIdAdapter.Adapt(PlayerId);
                return result;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var request = filterContext.HttpContext.Request;
            if (request.Url != null && !request.IsAjaxRequest())
            {
                _accessAuditer.Audit(request.Url.Host, request.Url.PathAndQuery, PlayerId);
            }

        }

        public ActionResult IsOnTheGameServer()
        {
            var isOnTheGameServer = false;
            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
                var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                if (serverCurrentInfo.Players.Any(x => x.PlayerId.Equals(PlayerId)))
                {
                    isOnTheGameServer = true;
                }
            }
            var result = Json(isOnTheGameServer, JsonRequestBehavior.AllowGet);
            return result;
        }

        public ActionResult GetContactQueries()
        {
            var queries = EndpointsController.ContactQueries.ContainsKey(PlayerId) ? EndpointsController.ContactQueries[PlayerId] : new List<string>();
            var result = Json(queries.Reverse(), JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}