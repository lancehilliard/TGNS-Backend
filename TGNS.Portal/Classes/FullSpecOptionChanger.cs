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
    public class FullSpecOptionChanger
    {
        readonly IFullSpecOptionsModelGetter _fullSpecOptionsModelGetter;

        public FullSpecOptionChanger()
        {
            _fullSpecOptionsModelGetter = new FullSpecOptionsOptionsModelGetter();
        }

        public void OptIn(long playerId)
        {
            var fullSpecOptionsModel = _fullSpecOptionsModelGetter.Get();
            if (fullSpecOptionsModel.EnrolledPlayerIds.All(x => x != playerId))
            {
                var enrolledPlayerIds = fullSpecOptionsModel.EnrolledPlayerIds.ToList();
                enrolledPlayerIds.Add(playerId);
                Update(new FullSpecOptionsModel(enrolledPlayerIds));
            }
        }

        public void OptOut(long playerId)
        {
            var fullSpecOptionsModel = _fullSpecOptionsModelGetter.Get();
            if (fullSpecOptionsModel.EnrolledPlayerIds.Any(x => x == playerId))
            {
                var enrolledPlayerIds = fullSpecOptionsModel.EnrolledPlayerIds.Where(x => x != playerId).ToList();
                Update(new FullSpecOptionsModel(enrolledPlayerIds));
            }
        }

        void Update(IFullSpecOptionsModel fullSpecOptionsModel)
        {
            var fullSpecOptions = new Dictionary<string, object>();
            fullSpecOptions.Add("enrolled", fullSpecOptionsModel.EnrolledPlayerIds);
            var datavalue = JsonConvert.SerializeObject(fullSpecOptions);
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE data SET datavalue = @DataValue WHERE datarealm = 'ns2' AND datatypename = 'fullspec';";
                    command.Prepare();
                    command.Parameters.AddWithValue("@DataValue", datavalue);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}