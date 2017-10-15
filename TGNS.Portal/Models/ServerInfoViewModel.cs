using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Models
{
    public class ServerInfoViewModel
    {
        public ServerInfoViewModel(IEnumerable<IServerCurrentInfo> serverCurrentInfos)
        {
            ServerCurrentInfos = serverCurrentInfos;
        }

        public IEnumerable<IServerCurrentInfo> ServerCurrentInfos { get; private set; }
    }
}