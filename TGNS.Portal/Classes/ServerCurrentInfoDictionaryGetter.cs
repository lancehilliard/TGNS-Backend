using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TGNS.Core.Commands;
using TGNS.Core.Domain;
using TGNS.Core.Security;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IServerCurrentInfoDictionaryGetter
    {
        IDictionary<string, object> Get(IServerModel serverModel);
    }

    public class ServerCurrentInfoDictionaryDictionaryGetter : IServerCurrentInfoDictionaryGetter
    {
        private readonly ServerAdminNetworkCredentialBuilder _serverAdminNetworkCredentialBuilder;

        public ServerCurrentInfoDictionaryDictionaryGetter()
        {
            _serverAdminNetworkCredentialBuilder = new ServerAdminNetworkCredentialBuilder();
        }

        public IDictionary<string, object> Get(IServerModel serverModel)
        {
            var result = new Dictionary<string, object> {{"success", true}};
            var serverAdminNetworkCredential = _serverAdminNetworkCredentialBuilder.Build();
            var webClient = new ServerQueryWebClient { Credentials = serverAdminNetworkCredential };
            try
            {
                var serverStatusResponse = webClient.DownloadString(serverModel.WebAdminBaseUrl);
                var serverStatus = JsonConvert.DeserializeObject<Dictionary<string, object>>(serverStatusResponse);
                foreach (var key in serverStatus.Keys)
                {
                    result.Add(key, serverStatus[key]);
                }
            }
            catch (Exception e)
            {
                result["success"] = false;
            }
            return result;
        } 
    }
}