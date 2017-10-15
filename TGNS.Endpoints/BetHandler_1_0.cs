using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Endpoints
{
    public class BetHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["a"]);
            return result;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var makerPlayerId = request["m"];
            var killerPlayerId = request["k"];
            var victimPlayerId = request["v"];
            var type = request["t"];
            var amount = request["a"];
            command.CommandText = "INSERT INTO bet_transactions (Realm, MakerPlayerId, KillerPlayerId, VictimPlayerId, Type, Amount) VALUES (@Realm, @MakerPlayerId, @KillerPlayerId, @VictimPlayerId, @Type, @Amount);";
            command.Prepare();
            command.Parameters.AddWithValue("@Realm", realmName);
            command.Parameters.AddWithValue("@MakerPlayerId", makerPlayerId);
            Action<string, string> addPlayerIdThatMightBeNull = (parameterName, parameterValue) =>
            {
                if (parameterValue.All(char.IsDigit))
                {
                    command.Parameters.AddWithValue(parameterName, parameterValue);
                }
                else
                {
                    command.Parameters.AddWithValue(parameterName, DBNull.Value);
                }
            };
            addPlayerIdThatMightBeNull("@KillerPlayerId", killerPlayerId);
            addPlayerIdThatMightBeNull("@VictimPlayerId", victimPlayerId);
            command.Parameters.AddWithValue("@Type", type);
            command.Parameters.AddWithValue("@Amount", amount);
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var amount = 0d;
            var playerId = request["i"];
            using (var command = new MySqlCommand { Connection = connection })
            {
                command.CommandText = "select COALESCE(SUM(Amount),0) as AmountSum from bet_transactions where MakerPlayerId = @PlayerId and Realm = @Realm;";
                command.Prepare();
                command.Parameters.AddWithValue("@Realm", realmName);
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        amount = Math.Floor(reader.GetDouble("AmountSum"));
                    }
                }
            }
            amount = amount <= 9999999999999 ? amount : 9999999999999;
            // var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", amount.ToString("F99").TrimEnd("0".ToCharArray()).Replace(CultureInfo.GetCultureInfo("en-US").NumberFormat.NumberDecimalSeparator, string.Empty) } }); // http://stackoverflow.com/a/7032578/116895 , http://stackoverflow.com/a/1884403/116895
            var result = JsonConvert.SerializeObject(new Dictionary<string, object> { { "success", true }, { "result", amount } });
            return result;
        }
    }
}