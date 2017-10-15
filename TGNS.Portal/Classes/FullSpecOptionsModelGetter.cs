using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IFullSpecOptionsModelGetter
    {
        IFullSpecOptionsModel Get();
    }

    public class FullSpecOptionsOptionsModelGetter : IFullSpecOptionsModelGetter
    {
        public IFullSpecOptionsModel Get()
        {
            string fullSpecJson = null;
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select datavalue from data where datatypename LIKE 'fullspec' and datarealm = 'ns2';";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fullSpecJson = reader.GetString("datavalue");
                        }
                    }
                }
            }
            var fullSpecData = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<long>>>(fullSpecJson);
            var result = new FullSpecOptionsModel(fullSpecData["enrolled"]);
            return result;
        } 
    }
}