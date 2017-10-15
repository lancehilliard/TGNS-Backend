using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Portal.Classes
{
    public interface IBalanceTotalGamesCountGetter
    {
        int? Get(long playerId);
    }

    public class BalanceTotalGamesCountGetter : IBalanceTotalGamesCountGetter
    {
        public int? Get(long playerId)
        {
            var result = default(int?);
            string balanceJson = null;
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select datavalue from data where datatypename = 'balance' and datarealm = 'ns2' and datarecordid = @PlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            balanceJson = reader.GetString("datavalue");
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(balanceJson))
            {
                var balanceData = JsonConvert.DeserializeObject<Dictionary<string, object>>(balanceJson);
                result = Convert.ToInt32(balanceData["total"]);
            }
            return result;
        }
    }
}