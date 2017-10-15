using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TGNS.Core.Data
{
    public interface IBkaDataParser
    {
        IBkaData Parse(string bkaJson);
    }

    public class BkaDataParser : IBkaDataParser
    {
        public IBkaData Parse(string bkaJson)
        {
            bkaJson = bkaJson.Replace(@"""", "\"");
            var bkaData = JsonConvert.DeserializeObject<Dictionary<string, object>>(bkaJson);
            var playerId = Convert.ToInt64(bkaData["steamId"]);
            var bka = bkaData["BKA"] as string;
            var playerSetInSeconds = bkaData.ContainsKey("BKAPlayerModifiedAtInSeconds") ? Convert.ToInt64(bkaData["BKAPlayerModifiedAtInSeconds"]) : 0;
            var result = new BkaData(playerId, bka, playerSetInSeconds);
            return result;
        }
    }
}