using TGNS.Core.Data;

namespace TGNS.Portal.Models
{
    public interface IDeletableGameRecording
    {
        IGame Game { get; }
        long PlayerId { get; }
        IBkaData BkaData { get; }
    }

    public class DeletableGameRecording : IDeletableGameRecording
    {
        public DeletableGameRecording(IGame game, long playerId, IBkaData bkaData)
        {
            Game = game;
            PlayerId = playerId;
            BkaData = bkaData;
        }

        public IGame Game { get; set; }
        public long PlayerId { get; set; }
        public IBkaData BkaData { get; }
    }
}