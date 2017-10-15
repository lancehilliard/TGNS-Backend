using System.IO;
using Renci.SshNet;

namespace TGNS.Core.Commands
{
    public interface ISftpFileUploader
    {
        void Upload(byte[] contents, string host, int port, string username, string password, string absolutePathWithFilename);
    }

    public class SftpFileUploader : ISftpFileUploader
    {
        public void Upload(byte[] contents, string host, int port, string username, string password, string absolutePathWithFilename)
        {
            using (var memoryStream = new MemoryStream(contents))
            {
                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    sftp.UploadFile(memoryStream, $"{absolutePathWithFilename}");
                }
            }
        }
    }
}