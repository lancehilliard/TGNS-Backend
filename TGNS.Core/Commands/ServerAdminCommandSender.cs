using System;
using System.Threading;
using System.Web;
using TGNS.Core.Security;

namespace TGNS.Core.Commands
{
    public interface IServerAdminCommandSender
    {
        bool Send(string serverAdminUrl, string userName, long playerId, string command, bool commandShouldBeAnnouncedOnGameServer);
    }

    public class ServerAdminCommandSender : IServerAdminCommandSender
    {
        readonly ServerAdminNetworkCredentialBuilder _serverAdminCredentialBuilder;

        public ServerAdminCommandSender()
        {
            _serverAdminCredentialBuilder = new ServerAdminNetworkCredentialBuilder();
        }

        public bool Send(string serverAdminUrl, string userName, long playerId, string command, bool commandShouldBeAnnouncedOnGameServer)
        {
            var result = true;
            var networkCredential = _serverAdminCredentialBuilder.Build();
            var webClient = new ServerQueryWebClient { Credentials = networkCredential };
            try
            {
                if (commandShouldBeAnnouncedOnGameServer)
                {
                    webClient.DownloadString(string.Format("{0}?command=Send&rcon={1}", serverAdminUrl, HttpUtility.UrlEncode(string.Format("sv_say {0} ({1}) performing '{2}' from TGNS Portal...", userName, playerId, command))));
                    Thread.Sleep(3000);
                }
                webClient.DownloadString(string.Format("{0}?command=Send&rcon={1}", serverAdminUrl, HttpUtility.UrlEncode(command)));
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }
    }
}