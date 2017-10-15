using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using Newtonsoft.Json;

namespace TGNS.Core.Data
{
    public interface IPlayerAdminChecker
    {
        bool IsAdmin(long playerId);
    }

    public class PlayerAdminChecker : IPlayerAdminChecker
    {
        public bool IsAdmin(long playerId)
        {
            var adminPlayerIds = HttpRuntime.Cache["AdminPlayerIds"] as List<long> ?? new List<long>();
            if (adminPlayerIds.Count == 0)
            {
                try
                {
                    var tgJsonFixer = new TgJsonFixer();
                    var webClient = new WebClient();
                    var adminsJson = webClient.DownloadString("URL_HERE");
                    var fixedAdminsJson = tgJsonFixer.Fix(adminsJson);
                    var adminsDictionaries = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(fixedAdminsJson);
                    adminPlayerIds.AddRange(adminsDictionaries.Select(adminsDictionary => Convert.ToInt64(adminsDictionary["id"] as object)));
                }
                catch (Exception e)
                {
                    var backupPortalAdmins = ConfigurationManager.AppSettings["BackupPortalAdmins"].Split(',').Select(x=> Convert.ToInt64(x));
                    adminPlayerIds.AddRange(backupPortalAdmins);
                }
                HttpRuntime.Cache.Insert("AdminPlayerIds", adminPlayerIds, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0));
            }
            var result = adminPlayerIds.Contains(playerId);
            return result;
        }
    }
}