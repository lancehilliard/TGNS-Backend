using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using TGNS.Core.Commands;

namespace TGNS.Portal.Classes
{
    public interface IServerMapCycler
    {
        bool Cycle(string userName, long playerId, string serverAdminUrl);
    }

    public class ServerMapCycler : IServerMapCycler
    {
        readonly IServerAdminCommandSender _serverAdminCommandSender;

        public ServerMapCycler()
        {
            _serverAdminCommandSender = new ServerAdminCommandSender();
        }

        public bool Cycle(string userName, long playerId, string serverAdminUrl)
        {
            return _serverAdminCommandSender.Send(serverAdminUrl, userName, playerId, "changemap ns2_veil", true);
        }
    }
}