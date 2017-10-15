using System.Collections.Generic;
using System.Linq;
using SteamKit2;
using TGNS.Core.Steam;

namespace TGNS.Core.Data
{
    public interface IPlayerIdAdapter
    {
        IEnumerable<SteamID> Adapt(IEnumerable<long> playerIds);
        SteamID Adapt(long playerId);
    }

    public class PlayerIdAdapter : IPlayerIdAdapter
    {
        private readonly ISteamIdConverter _steamIdConverter;
        public PlayerIdAdapter()
        {
            _steamIdConverter = new SteamIdConverter();
        }

        public IEnumerable<SteamID> Adapt(IEnumerable<long> playerIds)
        {
            var result = playerIds.Select(Adapt);
            return result;
        }

        public SteamID Adapt(long playerId)
        {
            var readableSteamIdFromNs2Id = _steamIdConverter.GetReadableSteamIdFromNs2Id(playerId);
            var result = new SteamID(readableSteamIdFromNs2Id);
            return result;
        }
    }
}