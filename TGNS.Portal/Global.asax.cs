using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentScheduler;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            TaskManager.Initialize(new ScheduledTaskRegistry());
        }

        protected void Application_AcquireRequestState()
        {
            var _playerIdGetter = new PlayerIdGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            var _playerAdminChecker = new PlayerAdminChecker();
            Action<long> SetIsAdmin = x => HttpContext.Current.Items["IsAdmin"] = _playerAdminChecker.IsAdmin(x);
            string userName = null;
            var playerId = default(long);
            if (string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
            {
                var serverPlayer = GetServerPlayer();
                if (serverPlayer != null)
                {
                    userName = string.Format("{0} (Game Server)", serverPlayer.PlayerName);
                    playerId = serverPlayer.PlayerId;
                    SetIsAdmin(playerId);
                }

                HttpContext.Current.Items["UserName"] = userName;
                HttpContext.Current.Items["PlayerId"] = playerId;
            }
            else
            {
                HttpContext.Current.Items["UserName"] = HttpContext.Current.User.Identity.Name;
                playerId = _playerIdGetter.Get(HttpContext.Current.User.Identity.Name);
                HttpContext.Current.Items["PlayerId"] = playerId;
                SetIsAdmin(playerId);
            }
            //if (HttpContext.Current.Session != null && Request.IsAuthenticated)
            //{
            //    var currentUserPlayerIdGetter = new CurrentUserPlayerIdGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            //    var currentUserPlayerId = currentUserPlayerIdGetter.Get();
            //    var playerAdminChecker = new PlayerAdminChecker();
            //    if (playerAdminChecker.IsAdmin(currentUserPlayerId))
            //    {
            //        HttpContext.Current.Items["IsAdmin"] = true;
            //    }
            //}
        }

        private IServerPlayer GetServerPlayer()
        {
            IServerPlayer result = null;
            var _serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            var _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
            var _serverGetter = new ServerGetter();
            var serverPlayers = new List<IServerPlayer>();
            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
                var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                serverPlayers.AddRange(serverCurrentInfo.Players);
            }
            var players = serverPlayers.Where(x => x.IpAddress.Equals(HttpContext.Current.Request.UserHostAddress)).ToList();
            if (players.Count() == 1)
            {
                result = players.First();
            }
            return result;
        }
    }
}
