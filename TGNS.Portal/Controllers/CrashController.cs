using System.Linq;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class CrashController : AuthenticatedController
    {
        private readonly IServerAdminCommandSender _serverAdminCommandSender;
        private readonly IServerCurrentInfoDictionaryGetter _serverCurrentInfoDictionaryGetter;
        private readonly IServerCurrentInfoAdapter _serverCurrentInfoAdapter;

        public CrashController()
        {
            _serverAdminCommandSender = new ServerAdminCommandSender();
            _serverCurrentInfoDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
            _serverCurrentInfoAdapter = new ServerCurrentInfoAdapter();
        }

        public ActionResult Kick()
        {
            var serverModels = _serverGetter.Get();
            foreach (var serverModel in serverModels)
            {
                var serverCurrentInfoDictionary = _serverCurrentInfoDictionaryGetter.Get(serverModel);
                var serverCurrentInfo = _serverCurrentInfoAdapter.Adapt(serverCurrentInfoDictionary);
                if (serverCurrentInfo.Players.Any(x => x.PlayerId.Equals(PlayerId)))
                {
                    _serverAdminCommandSender.Send(serverModel.WebAdminBaseUrl, UserName, PlayerId, $"sh_portalkick {PlayerId} Crashed", false);
                }
            }
            return View("Kicked");
        }
    }
}