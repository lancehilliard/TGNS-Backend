using System;
using System.Net;

namespace TGNS.Core.Commands
{
    public class ServerQueryWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            var result = base.GetWebRequest(uri);
            if (result != null)
            {
                result.Timeout = 5000;
            }
            return result;
        }
    }
}