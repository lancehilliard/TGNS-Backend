using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IServerCurrentInfoAdapter
    {
        IServerCurrentInfo Adapt(IDictionary<string, object> serverCurrentInfo);
    }

    public class ServerCurrentInfoAdapter : IServerCurrentInfoAdapter
    {
        private readonly IServerPlayerAdapter _serverPlayerAdapter;

        public ServerCurrentInfoAdapter()
        {
            _serverPlayerAdapter = new ServerPlayerAdapter();
        }

        public IServerCurrentInfo Adapt(IDictionary<string, object> serverCurrentInfo)
        {
            var mapName = string.Empty;
            var serverName = string.Empty;
            var players = new List<IServerPlayer>();
            if (Equals(serverCurrentInfo["success"], true))
            {
                var playerList = (JArray)serverCurrentInfo["player_list"];
                var playerDictionaries = playerList.ToObject<IEnumerable<Dictionary<string, object>>>();
                players.AddRange(playerDictionaries.Select(_serverPlayerAdapter.Adapt));
                mapName = Convert.ToString(serverCurrentInfo["map"]);
                serverName = Convert.ToString(serverCurrentInfo["server_name"]);
            }
            var result = new ServerCurrentInfo(mapName, players, serverName);
            return result;
        }
    }
}