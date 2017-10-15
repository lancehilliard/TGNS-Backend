using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IBanAdapter
    {
        BanViewModel Adapt(IBan ban);
    }

    public class BanAdapter : IBanAdapter
    {
        public BanViewModel Adapt(IBan ban)
        {
            var result = new BanViewModel(ban.UnbanTime, ban.PlayerName, ban.CreatorName, ban.DurationInSeconds, ban.Reason, ban.PlayerId);
            return result;
        }
    }
}