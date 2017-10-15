using System.Collections.Generic;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class PreferredHandler_1_0 : Handler
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
            var preferredEntriesGetter = new PreferredEntriesGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            var preferredEntries = preferredEntriesGetter.Get(realmName);
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", preferredEntries } });
            return result;
        }
    }
}