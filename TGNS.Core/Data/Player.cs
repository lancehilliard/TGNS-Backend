namespace TGNS.Core.Data
{
    public interface IPlayer
    {
        string Name { get; }
        long PlayerId { get; }
    }

    public class Player : IPlayer
    {
        public Player(string name, long playerId)
        {
            Name = name;
            PlayerId = playerId;
        }

        public string Name { get; private set; }
        public long PlayerId { get; private set; }
    }
}