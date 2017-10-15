namespace TGNS.Core.Domain
{
    public interface IBucketPlayer
    {
        string Name { get; }
        long Id { get; }
    }

    public class BucketPlayer : IBucketPlayer
    {
        public BucketPlayer(string name, long id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; private set; }
        public long Id { get; private set; }
    }
}