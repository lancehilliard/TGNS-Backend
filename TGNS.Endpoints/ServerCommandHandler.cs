using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Commands;
using TGNS.Core.Domain;

namespace TGNS.Endpoints
{
    public class ServerCommandHandler_1_0 : Handler
    {
        private ServerAdminCommandSender _serverAdminCommandSender;
        private readonly IServerGetter _serverGetter;
        private IServerProcessCommandSender _serverProcessCommandSender;
        public ServerCommandHandler_1_0()
        {
            _serverAdminCommandSender = new ServerAdminCommandSender();
            _serverGetter = new ServerGetter();
            _serverProcessCommandSender = new ServerProcessCommandSender();
        }

        protected override bool IsReadRequest(HttpRequest request)
        {
            return true;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var serverSimpleName = Convert.ToString(request["s"]);
            var shouldAnnounce = Convert.ToBoolean(request["a"]);
            var command = Convert.ToString(request["c"]);
            var timeToRestart = Convert.ToInt32(request["t"]);
            var serverModels = _serverGetter.Get();
            var serverModel = serverModels.Single(x=>x.Name.Equals(serverSimpleName));
            _serverAdminCommandSender.Send(serverModel.WebAdminBaseUrl, "Portal", 0, command, shouldAnnounce);
            if (timeToRestart > 0)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Thread.Sleep(timeToRestart*1000);
                    _serverProcessCommandSender.Restart(serverModel.InstanceIndex);
                }).Start();
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true } });
            return result;
        }
    }
}