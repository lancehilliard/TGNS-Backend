using System.Configuration;
using System.Net;

namespace TGNS.Core.Security
{
    public interface IServerAdminNetworkCredentialBuilder
    {
        NetworkCredential Build();
    }

    public class ServerAdminNetworkCredentialBuilder : IServerAdminNetworkCredentialBuilder
    {
        public NetworkCredential Build()
        {
            var webAdminUsername = ConfigurationManager.AppSettings["WebAdminUsername"];
            var webAdminPassword = ConfigurationManager.AppSettings["WebAdminPassword"];
            var result = new NetworkCredential(webAdminUsername, webAdminPassword);
            return result;
        }
    }
}