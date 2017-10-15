using System;
using System.Collections.Generic;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IServerPlayerAdapter
    {
        IServerPlayer Adapt(IDictionary<string, object> playerDictionary);
    }

    public class ServerPlayerAdapter : IServerPlayerAdapter
    {
        public IServerPlayer Adapt(IDictionary<string, object> playerDictionary)
        {
            var result = new ServerPlayer(Convert.ToInt32(playerDictionary["steamid"]), Convert.ToString(playerDictionary["name"]), Convert.ToInt32(playerDictionary["team"]), Convert.ToBoolean(playerDictionary["isbot"]), Convert.ToInt32(playerDictionary["score"]), Convert.ToDecimal(playerDictionary["resources"]), Convert.ToString(playerDictionary["ipaddress"]), Convert.ToBoolean(playerDictionary["iscomm"]));
            return result;
        }
    }
}