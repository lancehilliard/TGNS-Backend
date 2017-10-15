using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace TGNS.Endpoints
{
    public class DataHandler_1_0 : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);
            context.Response.ContentType = "application/json";
            try
            {
                var queryString = context.Request.QueryString;
                var password = queryString["p"];
                if (string.Equals(password, ConfigurationManager.AppSettings["handlerPassword"]))
                {
                    var dataRealm = queryString["r"];
                    var dataTypeName = queryString["d"];
                    var dataRecordId = queryString["i"];
                    var dataValue = queryString["v"];

                    using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
                    {
                        connection.Open();
                        using (var command = new MySqlCommand { Connection = connection })
                        {
                            if (!string.IsNullOrWhiteSpace(dataValue))
                            {
                                command.CommandText = "INSERT INTO data (DataRealm, DataTypeName, DataRecordId, DataValue) VALUES (@DataRealm, @DataTypeName, @DataRecordId, @DataValue) ON DUPLICATE KEY UPDATE DataValue=@DataValue;";
                                command.Prepare();
                                command.Parameters.AddWithValue("@DataRealm", dataRealm);
                                command.Parameters.AddWithValue("@DataTypeName", dataTypeName);
                                command.Parameters.AddWithValue("@DataRecordId", dataRecordId);
                                command.Parameters.AddWithValue("@DataValue", dataValue);
                                command.ExecuteNonQuery();
                                context.Response.Write(@"{""success"": true}");
                            }
                            else
                            {
                                command.CommandText = "SELECT DataValue FROM data WHERE DataRealm=@DataRealm AND DataTypeName=@DataTypeName AND DataRecordId=@DataRecordId;";
                                command.Prepare();
                                command.Parameters.AddWithValue("@DataRealm", dataRealm);
                                command.Parameters.AddWithValue("@DataTypeName", dataTypeName);
                                command.Parameters.AddWithValue("@DataRecordId", dataRecordId);
                                var reader = command.ExecuteReader();
                                while (reader.Read())
                                {
                                    dataValue = reader.GetString("DataValue");
                                }
                                context.Response.Write(string.Format(@"{{""success"": true, ""value"": {0}}}", string.IsNullOrWhiteSpace(dataValue) ? "{}" : dataValue));
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Handler password incorrect.");
                }
            }
            catch (Exception exception)
            {
                context.Response.Write(string.Format(@"{{""success"": false, ""message"": ""{0}"", ""stacktrace"": ""{1}""}}", exception.Message, exception.StackTrace));
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }

   public class DataHandler_1_1 : Handler
    {
       protected override bool IsReadRequest(HttpRequest request)
       {
           var result = string.IsNullOrWhiteSpace(request["v"]);
           return result;
       }

       protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
       {
           var dataRealm = request["r"];
           var dataTypeName = request["d"];
           var dataRecordId = request["i"];
           var dataValue = request["v"];

           command.CommandText = "INSERT INTO data (DataRealm, DataTypeName, DataRecordId, DataValue) VALUES (@DataRealm, @DataTypeName, @DataRecordId, @DataValue) ON DUPLICATE KEY UPDATE DataValue=@DataValue;";
           command.Prepare();
           command.Parameters.AddWithValue("@DataRealm", dataRealm);
           command.Parameters.AddWithValue("@DataTypeName", dataTypeName);
           command.Parameters.AddWithValue("@DataRecordId", dataRecordId);
           command.Parameters.AddWithValue("@DataValue", dataValue);
           command.ExecuteNonQuery();
       }

       // http://208.43.76.155:4155/gen/v1/data?p=PASSWORD_HERE&r=ns2&d=notedplayers&i=notedplayers
       protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection) {
           var dataTypeName = request["d"];
           var dataRecordId = request["i"];

           var dataValue = default(string);
           using (var command = new MySqlCommand { Connection = connection })
           {
               command.CommandText = "SELECT DataValue FROM data WHERE DataRealm=@DataRealm AND DataTypeName=@DataTypeName AND DataRecordId=@DataRecordId;";
               command.Prepare();
               command.Parameters.AddWithValue("@DataRealm", realmName);
               command.Parameters.AddWithValue("@DataTypeName", dataTypeName);
               command.Parameters.AddWithValue("@DataRecordId", dataRecordId);
               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       dataValue = reader.GetString("DataValue");
                   }
               }
           }
           var result = string.Format(@"{{""success"": true, ""value"": {0}}}", string.IsNullOrWhiteSpace(dataValue) ? "{}" : dataValue);
           return result;
       }
    }
}
