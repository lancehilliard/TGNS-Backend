using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IServerAdapter
    {
        IEnumerable<ServerViewModel> Adapt(IEnumerable<IServerModel> servers);
        ServerViewModel Adapt(IServerModel server);
    }

    public class ServerAdapter : IServerAdapter
    {
        public IEnumerable<ServerViewModel> Adapt(IEnumerable<IServerModel> servers)
        {
            return servers.Select(Adapt);
        }
        
        public ServerViewModel Adapt(IServerModel server)
        {
            var result = new ServerViewModel(server.ID, server.Name, server.WebAdminBaseUrl);
            return result;
        }
    }
}