using System;
using System.Configuration;
using System.Data.Entity.Migrations.Model;
using System.Text;
using TGNS.Core.Commands;
using TGNS.Core.Messaging;

namespace TGNS.Portal.Classes
{
    public class ServerAdminJsonUpdater
    {
        private readonly IMessagePusher _messagePusher;
        public ServerAdminJsonUpdater()
        {
            _messagePusher = new PushbulletMessagePusher(ConfigurationManager.AppSettings["PushbulletEncodedAuthKey"]);
        }

        public void Update()
        {
            var parseErrorNotifications = (int?)System.Runtime.Caching.MemoryCache.Default["ParseErrorNotifications"] ?? 0;
            try
            {
                var serverAdminJsonCreator = new ServerAdminJsonCreator();
                var serverAdminJson = serverAdminJsonCreator.Create();
                var contents = Encoding.UTF8.GetBytes(serverAdminJson);
                var host = ConfigurationManager.AppSettings["FtpHost"];
                var port = Convert.ToInt32(ConfigurationManager.AppSettings["FtpPort"]);
                var username = ConfigurationManager.AppSettings["TgUsername"];
                var password = ConfigurationManager.AppSettings["TgPassword"];
                var path = ConfigurationManager.AppSettings["ConfigDirFtpPath"];
                var sftpFileUploader = new SftpFileUploader();
                sftpFileUploader.Upload(contents, host, port, username, password, $"{path}/ServerAdmin.json");
                System.Runtime.Caching.MemoryCache.Default["ParseErrorNotifications"] = 0;
            }
            catch (Exception e)
            {
                if (parseErrorNotifications > 0 && parseErrorNotifications <= 3)
                {
                    _messagePusher.Push("tgns-admin", "Update Error", $"Message: {e.Message}");
                }
                System.Runtime.Caching.MemoryCache.Default["ParseErrorNotifications"] = parseErrorNotifications + 1;
            }
        }
    }
}