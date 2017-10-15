using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGNS.Core.Data
{
    public interface IPlayerIdentity
    {
        long PlayerId { get; }
        IEnumerable<string> Aliases { get; }
    }

    public class PlayerIdentity : IPlayerIdentity
    {
        public PlayerIdentity(long playerId, IEnumerable<string> aliases)
        {
            PlayerId = playerId;
            Aliases = aliases;
        }

        public long PlayerId { get; private set; }
        public IEnumerable<string> Aliases { get; private set; }

    }
}
