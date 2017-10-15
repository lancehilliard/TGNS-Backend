using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class BetterKnownAsHandler_1_0 : Handler
    {
        private readonly IBkaDataParser _bkaDataParser;

        public BetterKnownAsHandler_1_0()
        {
            _bkaDataParser = new BkaDataParser();
        }

        protected override bool IsReadRequest(HttpRequest request)
        {
            return true;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var bkaDatas = new List<BkaData>();
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = @"select DataValue from data where datatypename = 'bka' and datavalue like '%""BKA"":%' and datavalue not like '%""BKA"":""""%' and datavalue not like '%""BKA"": """"%' and datarealm = @BkaRealm";
                command.Prepare();
                command.Parameters.AddWithValue("@BkaRealm", realmName);

                string query = command.Parameters.Cast<MySqlParameter>().Aggregate(command.CommandText, (current, p) => current.Replace(p.ParameterName, p.Value.ToString()));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bkaData = _bkaDataParser.Parse(reader.GetString("DataValue"));
                        bkaDatas.Add(new BkaData(bkaData.PlayerId, bkaData.Bka, bkaData.PlayerSetGmtInSeconds));
                    }
                }
            }
            var data = bkaDatas.Select(x=>new {id=x.PlayerId,bka=x.Bka});
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", data } });
            return result;
        }
    }
}