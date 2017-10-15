using System.Linq;
using TGNS.Core.Steam;

namespace TGNS.Portal.Classes
{
    public interface IIdentityInitializer
    {
        void Initialize(string username, string providerKey);
    }

    public class IdentityInitializer : IIdentityInitializer
    {
        public void Initialize(string username, string providerKey)
        {
            var steamIdConverter = new SteamIdConverter();
            var portalUserCreator = new PortalUserCreator();
            var providerKeyParts = providerKey.Split('/');
            var steamId = providerKeyParts.Last();
            var ns2Id = steamIdConverter.GetNs2IdFrom64BitSteamId(steamId);
            portalUserCreator.Create(ns2Id, username);
        }
    }
}