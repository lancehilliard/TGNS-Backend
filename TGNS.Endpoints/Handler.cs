using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints
{
    public abstract class Handler : IHttpHandler
    {
        abstract protected bool IsReadRequest(HttpRequest request);
        abstract protected void Write(string realmName, HttpRequest request, MySqlCommand command);
        abstract protected string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);
            context.Response.ContentType = "application/json";
            try
            {
                var request = context.Request;
                var password = request["p"];
                if (string.Equals(password, ConfigurationManager.AppSettings["handlerPassword"]))
                {
                    var realmName = request["r"];
                    using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
                    {
                        connection.Open();
                        using (var command = new MySqlCommand())
                        {
                            command.Connection = connection;
                            if (IsReadRequest(request))
                            {
                                var responseJson = GetResponseJson(realmName, request, connection);
                                context.Response.Write(responseJson);
                            }
                            else
                            {
                                Write(realmName, request, command);
                                context.Response.Write(@"{""success"": true}");
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
                var message = string.Format("Exception message: {0}", exception.Message);
                if (exception.InnerException != null)
                {
                    message = string.Format("{0} | Inner Exception message: {1}", message, exception.InnerException.Message);
                }
                context.Response.Write(string.Format(@"{{""success"": false, ""msg"": ""{0}"", ""stacktrace"": ""{1}""}}", message, exception.StackTrace.Replace(Environment.NewLine, @" \ ")));
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}