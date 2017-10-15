using System.Configuration;
using System.Web.Mvc;
using TGNS.Core.Commands;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class ServerCommandsController : AdminController
    {
        private readonly IServerProcessCommandSender _serverProcessCommandSender;

        public ServerCommandsController()
        {
            _serverProcessCommandSender = new ServerProcessCommandSender();
        }

        public ActionResult Stop(int id)
        {
            _serverProcessCommandSender.Stop(id);
            TempData["Success"] = "Server stopped.";
            return RedirectToAction("Index", "Admins");
        }

        public ActionResult Update(int id)
        {
            _serverProcessCommandSender.Update(id);
            TempData["Success"] = "Server updated and restarted.";
            return RedirectToAction("Index", "Admins");
        }

        public ActionResult Start(int id)
        {
            _serverProcessCommandSender.Start(id);
            TempData["Success"] = "Server started.";
            return RedirectToAction("Index", "Admins");
        }

        public ActionResult Restart(int id)
        {
            _serverProcessCommandSender.Restart(id);
            TempData["Success"] = "Server restarted.";
            return RedirectToAction("Index", "Admins");
        }
    }
}