using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class BotNamesHandler_1_0 : Handler
    {
        private readonly IBkaDataParser _bkaDataParser;

        public BotNamesHandler_1_0()
        {
            _bkaDataParser = new BkaDataParser();
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
            var names = new List<string>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"select x.PlayerId, d.DataValue
                    from (select COUNT(DISTINCT EXTRACT( YEAR_MONTH FROM RowCreated )) AS SupportingMemberMonths,
                        PlayerId
                    from games_players
                    where SupportingMember
                    group by 2) x
                    inner join data d on x.playerid = d.DataRecordId and d.datatypename	= 'bka' and d.datarealm = 'ns2' and d.datavalue not like '%""BKA"":""""%' and d.datavalue not like '%""BKA"": """"%' and x.SupportingMemberMonths >= 12
                    order by SupportingMemberMonths DESC";
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bkaData = _bkaDataParser.Parse(reader.GetString("DataValue"));
                        if (!string.IsNullOrWhiteSpace(bkaData?.Bka))
                        {
                            names.Add(bkaData.Bka);
                        }
                    }
                }
            }
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "names", names } });
            return result;
        }
    }
}