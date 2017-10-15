using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Portal.Classes
{
    public interface ICronUsersGetter
    {
        IEnumerable<IPlayer> Get(string cronUrl);
    }

    public class CronUsersGetter : ICronUsersGetter
    {
        public static void Main()
        {
            //var cronUsersGetter = new CronUsersGetter();
            //var cronUsers = cronUsersGetter.Get("URL_HERE");
        }

        public IEnumerable<IPlayer> Get(string cronUrl)
        {
            var tgJsonFixer = new TgJsonFixer();
            var webClient = new WebClient();
            var usersJson = webClient.DownloadString(cronUrl);
            var fixedUsersJson = tgJsonFixer.Fix(usersJson);
            var usersDictionaries = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(fixedUsersJson);
            var result = usersDictionaries.Select(userDictionary => new Player(userDictionary["name"] as string, Convert.ToInt64(userDictionary["id"])));
            return result;
        }
    }
}