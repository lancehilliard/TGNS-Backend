using Renci.SshNet;

namespace TGNS.Core.Commands
{
    public interface ISshCommandSender
    {
        void ExecuteCommand(string commandText);
    }

    public class SshCommandSender : ISshCommandSender
    {
        private readonly string _sshHost;
        private readonly int _sshPort;
        private readonly string _sshUsername;
        private readonly string _sshPassword;

        public SshCommandSender(string sshHost, int sshPort, string sshUsername, string sshPassword)
        {
            _sshHost = sshHost;
            _sshPort = sshPort;
            _sshUsername = sshUsername;
            _sshPassword = sshPassword;
        }

        public void ExecuteCommand(string commandText)
        {
            using (var client = new SshClient(_sshHost, _sshPort, _sshUsername, _sshPassword))
            {
                client.Connect();
                client.RunCommand(commandText);
                client.Disconnect();
            }
        }

    }
}