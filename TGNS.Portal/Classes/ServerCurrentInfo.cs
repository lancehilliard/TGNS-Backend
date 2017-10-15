using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IServerCurrentInfo
    {
        string MapName { get; }
        IEnumerable<IServerPlayer> Players { get; }
        string ServerName { get; }
    }

    public class ServerCurrentInfo : IServerCurrentInfo
    {
        public ServerCurrentInfo(string mapName, IEnumerable<IServerPlayer> players, string serverName)
        {
            MapName = mapName;
            Players = players;
            ServerName = serverName;
        }

        public string MapName { get; private set; }
        public IEnumerable<IServerPlayer> Players { get; private set; }
        public string ServerName { get; private set; }
    }
}