using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Models
{
    public class ServerViewModel
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string WebAdminBaseUrl { get; private set; }

        public ServerViewModel(int id, string name, string webAdminBaseUrl)
        {
            ID = id;
            Name = name;
            WebAdminBaseUrl = webAdminBaseUrl;
        }
    }
}