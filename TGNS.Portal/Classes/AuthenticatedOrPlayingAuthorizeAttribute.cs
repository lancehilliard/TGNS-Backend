using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public class AuthenticatedOrPlayingAuthorizeAttribute : AuthorizeAttribute
    {
        //private readonly IServerGetter _serverGetter;
        //private readonly ServerCurrentInfoDictionaryDictionaryGetter _serverCurrentInfoDictionaryGetter;
        //private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;
        //private readonly IPlayerIdGetter _playerIdGetter;
        //private readonly ICurrentUserPlayerIdGetter _currentUserPlayerIdGetter;
        //private readonly IPlayerAdminChecker _playerAdminChecker;

        public AuthenticatedOrPlayingAuthorizeAttribute()
        {
            //_serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            //_serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
            //_serverGetter = new ServerGetter();
            //_playerIdGetter = new PlayerIdGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            //_currentUserPlayerIdGetter = new CurrentUserPlayerIdGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            //_playerAdminChecker = new PlayerAdminChecker();
        }

        //void SetIsAdmin(long playerId)
        //{
        //    HttpContext.Current.Items["IsAdmin"] = _playerAdminChecker.IsAdmin(playerId);
        //}

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //string userName = null;
            //var playerId = default(int);
            //if (string.IsNullOrWhiteSpace(httpContext.User.Identity.Name))
            //{
            //    var serverPlayer = GetServerPlayer();
            //    if (serverPlayer != null)
            //    {
            //        userName = serverPlayer.PlayerName;
            //        playerId = serverPlayer.PlayerId;
            //        SetIsAdmin(playerId);
            //    }
            //}
            //else
            //{
            //    userName = httpContext.User.Identity.Name;
            //    playerId = _playerIdGetter.Get(userName);
            //    SetIsAdmin(playerId);
            //}

            //httpContext.Items["UserName"] = userName;
            //httpContext.Items["PlayerId"] = playerId;
            //var result = !string.IsNullOrWhiteSpace(userName);
            //return result;
            var result = !string.IsNullOrWhiteSpace(httpContext.Items["UserName"] as string);
            return result;
        }

        //private IServerPlayer GetServerPlayer()
        //{
        //    IServerPlayer result = null;
        //    var serverModels = _serverGetter.Get();
        //    foreach (var serverModel in serverModels)
        //    {
        //        var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
        //        var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
        //        var serverPlayers = serverCurrentInfo.Players.ToList();
        //        serverPlayers.ForEach(delegate(IServerPlayer player)
        //        {
        //            var requestHostAddress = "75.64.63.168"; // httpContext.Request.UserHostAddress;
        //            if (result == null && player.IpAddress.Equals(requestHostAddress))
        //            {
        //                result = player;
        //            }
        //        });
        //    }
        //    return result;
        //}
    }
}