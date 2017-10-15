using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IServerCurrentPlayerCountGetter
    {
        int? Get(IServerModel serverModel);
    }

    public class ServerCurrentPlayerCountGetter : IServerCurrentPlayerCountGetter
    {
        private readonly ServerCurrentInfoDictionaryDictionaryGetter _serverCurrentInfoDictionaryDictionaryGetter;

        public ServerCurrentPlayerCountGetter()
        {
            _serverCurrentInfoDictionaryDictionaryGetter = new ServerCurrentInfoDictionaryDictionaryGetter();
        }

        public int? Get(IServerModel serverModel)
        {
            var result = default(int?);
            var info = _serverCurrentInfoDictionaryDictionaryGetter.Get(serverModel);
            if (Equals(info["success"], true))
            {
                result = Convert.ToInt32(info["players_online"]);
            }
            return result;
        }
    }
}