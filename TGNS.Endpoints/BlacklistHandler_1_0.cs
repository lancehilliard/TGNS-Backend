using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;
using TGNS.Core.Domain;

namespace TGNS.Endpoints
{
    public class BlacklistHandler_1_0 : Handler
    {
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
            var blacklistEntriesGetter = new BlacklistEntriesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            var blacklistEntries = blacklistEntriesGetter.Get(realmName);
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", blacklistEntries } });
            return result;
        }
    }
}