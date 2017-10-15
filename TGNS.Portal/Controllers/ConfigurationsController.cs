using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Commands;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Classes;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class ConfigurationsController : AdminController
    {
        private readonly IPluginConfigurationGetter _pluginConfigurationGetter;
        private readonly IPluginConfigurationSetter _pluginConfigurationSetter;

        public ConfigurationsController()
        {
            _pluginConfigurationGetter = new PluginConfigurationGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            _pluginConfigurationSetter = new PluginConfigurationSetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        [HttpGet]
        public ActionResult Manage(PluginsViewModel viewModel)
        {
            //var pluginsViewModel = new PluginsViewModel();
            var pluginConfigurationModels = _pluginConfigurationGetter.Get("ns2", "ns2").OrderBy(x => x.Name.EndsWith(".json")).ThenBy(x=>x.Name);
            var selectListItems = pluginConfigurationModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Name });
            viewModel.PluginNames = selectListItems;
            return View(viewModel);
        }

        [HttpGet]
        public PartialViewResult LoadConfig(string selectedPluginName)
        {
            var pluginsViewModel = new PluginsViewModel();
            var pluginConfigurationModels = _pluginConfigurationGetter.Get("ns2", "ns2");
            var selectListItems = pluginConfigurationModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Name });
            pluginsViewModel.PluginNames = selectListItems;
            var selectedPluginConfigurationModel = pluginConfigurationModels.Single(x => x.Name == selectedPluginName);
            pluginsViewModel.SelectedPluginName = selectedPluginConfigurationModel.Name;
            pluginsViewModel.PluginConfigJson = selectedPluginConfigurationModel.ConfigJson;
            return PartialView("_EditPluginConfig", pluginsViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditConfig(PluginsViewModel viewModel)
        {
            var message = string.Empty;
            var isError = false;
            var stacktrace = string.Empty;
            try
            {
                JsonConvert.DeserializeObject(viewModel.PluginConfigJson);
            }
            catch (Exception ex)
            {
                isError = true;
                message = "Config is not valid JSON. Correct JSON or Undo. Save operation aborted.";
                stacktrace = ex.StackTrace;
            }
            if (!isError)
            {
                _pluginConfigurationSetter.Set("ns2", "ns2", viewModel.SelectedPluginName, viewModel.PluginConfigJson);
                if (viewModel.SelectedPluginName.EndsWith(".json"))
                {
                    var contents = Encoding.UTF8.GetBytes(viewModel.PluginConfigJson);
                    var host = ConfigurationManager.AppSettings["FtpHost"];
                    var port = Convert.ToInt32(ConfigurationManager.AppSettings["FtpPort"]);
                    var username = ConfigurationManager.AppSettings["TgUsername"];
                    var password = ConfigurationManager.AppSettings["TgPassword"];
                    var path = ConfigurationManager.AppSettings["ConfigDirFtpPath"];
                    var sftpFileUploader = new SftpFileUploader();
                    sftpFileUploader.Upload(contents, host, port, username, password, $"{path}/{viewModel.SelectedPluginName}");
                    //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                    //FtpUrlTemplates.ForEach(x=>
                    //{
                    //    var contents = Encoding.UTF8.GetBytes(viewModel.PluginConfigJson);
                    //    var request = (FtpWebRequest)WebRequest.Create(string.Format(x, viewModel.SelectedPluginName));
                    //    request.Method = WebRequestMethods.Ftp.UploadFile;
                    //    request.EnableSsl = true;
                    //    request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["WebAdminUsername"], ConfigurationManager.AppSettings["WebAdminPassword"]);
                    //    request.ContentLength = contents.Length;
                    //    using (var requestStream = request.GetRequestStream())
                    //    {
                    //        requestStream.Write(contents, 0, contents.Length);
                    //    }
                    //    using (var response = (FtpWebResponse) request.GetResponse())
                    //    {
                    //        //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                    //    }
                    //});
                }
                message = string.Format("{0} plugin config saved.", viewModel.SelectedPluginName);
            }
            return Json(new {isError = isError, message = message, stacktrace = stacktrace});
        }
	}

    public interface IPluginConfigurationGetter
    {
        IEnumerable<IPluginConfigurationModel> Get(string realmName, string gameModeName);
    }

    public interface IPluginConfigurationSetter
    {
        void Set(string realmName, string gameModeName, string pluginName, string pluginConfigJson);
    }

    public class PluginConfigurationSetter : DataAccessor, IPluginConfigurationSetter
    {
        public PluginConfigurationSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(string realmName, string gameModeName, string pluginName, string pluginConfigJson)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"update plugins set pluginconfig=@PluginConfigJson where pluginrealm = @PluginRealm and plugingamemode = @PluginGameMode and pluginname = @PluginName;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PluginRealm", realmName);
                    command.Parameters.AddWithValue("@PluginGameMode", gameModeName);
                    command.Parameters.AddWithValue("@PluginName", pluginName);
                    command.Parameters.AddWithValue("@PluginConfigJson", pluginConfigJson);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class PluginConfigurationGetter : DataAccessor, IPluginConfigurationGetter
    {
        public PluginConfigurationGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IPluginConfigurationModel> Get(string realmName, string gameModeName)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT PluginName, PluginConfig FROM plugins WHERE PluginRealm=@PluginRealm AND PluginGameMode=@PluginGameMode;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PluginRealm", realmName);
                    command.Parameters.AddWithValue("@PluginGameMode", gameModeName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString("PluginName");
                            var configJson = reader.GetString("PluginConfig");
                            var result = new PluginConfigurationModel(name, configJson);
                            yield return result;
                        }
                    }
                }
            }
        }
    }

    public interface IPluginConfigurationModel
    {
        string Name { get; }
        string ConfigJson { get; }
    }

    public class PluginConfigurationModel : IPluginConfigurationModel
    {
        public PluginConfigurationModel(string name, string configJson)
        {
            Name = name;
            ConfigJson = configJson;
        }

        public string Name { get; private set; }
        public string ConfigJson { get; private set; }
    }
}