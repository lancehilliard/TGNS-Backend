using System;
using System.Configuration;

namespace TGNS.Core.Commands
{
    public interface IServerProcessCommandSender
    {
        void Stop(int serverIndex);
        void Start(int serverIndex);
        void Restart(int serverIndex);
        void Update(int serverIndex);
    }

    public class ServerProcessCommandSender : IServerProcessCommandSender
    {
        private readonly ISshCommandSender _sshCommandSender;

        public ServerProcessCommandSender()
        {
            _sshCommandSender = new SshCommandSender(SshHost, SshPort, TgUsername, TgPassword);
        }

        //string GetCommandTargetDescriptor(int serverIndex)
        //{
        //    return $"ns2 lounge {serverIndex}";
        //}

        string GetStopCommandText(int serverIndex)
        {
            return "ocs_service_stop";
        }

        string GetStartCommandText(int serverIndex)
        {
            return "ocs_service_run";
        }
        string GetUpdateCommandText(int serverIndex)
        {
            return "ocs_service_update";
        }

        string GetRestartCommandText(int serverIndex)
        {
            return "ocs_service_restart";
        }

        string SshHost => ConfigurationManager.AppSettings["SshHost"];
        int SshPort => Convert.ToInt32(ConfigurationManager.AppSettings["SshPort"]);
        string TgUsername => ConfigurationManager.AppSettings["TgUsername"];
        string TgPassword => ConfigurationManager.AppSettings["TgPassword"];

        public void Stop(int serverIndex)
        {
            _sshCommandSender.ExecuteCommand(GetStopCommandText(serverIndex));
        }

        public void Start(int serverIndex)
        {
            _sshCommandSender.ExecuteCommand(GetStartCommandText(serverIndex));
        }

        public void Restart(int serverIndex)
        {
            _sshCommandSender.ExecuteCommand(GetRestartCommandText(serverIndex));
        }

        public void Update(int serverIndex)
        {
            _sshCommandSender.ExecuteCommand(GetUpdateCommandText(serverIndex));
        }
    }
}