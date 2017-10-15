namespace TGNS.Core.Domain
{
    public interface IServerModel
    {
        int ID { get; }
        string Name { get; }
        string WebAdminBaseUrl { get; }
        int InstanceIndex { get; }
    }

    public class ServerModel : IServerModel
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string WebAdminBaseUrl { get; private set; }
        public int InstanceIndex { get; private set; }

        public ServerModel(int id, string name, string webAdminBaseUrl, int instanceIndex)
        {
            ID = id;
            Name = name;
            WebAdminBaseUrl = webAdminBaseUrl;
            InstanceIndex = instanceIndex;
        }
    }
}