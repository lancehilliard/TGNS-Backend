using System;

namespace TGNS.Core.Steam
{
    public interface ISteamIdConverter
    {
        long GetNs2IdFrom64BitSteamId(string input);
        string GetReadableSteamIdFromNs2Id(long steamIdNumber);
        long GetSteamCommunityProfileIdFromNs2Id(long ns2id);
        long GetSteamCommunityProfileIdFromReadableSteamId(string steamId);
    }

    public class SteamIdConverter : ISteamIdConverter
    {
        public long GetNs2IdFrom64BitSteamId(string input)
        {
            var steamId = GetSteamID(Convert.ToInt64(input));
            var result = GetNs2Id(steamId);
            return result;
        }

        private int GetNs2Id(string steamId)
        {
            var n=steamId.Split(':');
            var authServer = Convert.ToInt32(n[1]);
            var authId = Convert.ToInt32(n[2]);
            int result = authServer == 0 ? authId * 2 : (authId * 2)+1;
            return result;
        }

        private string GetCommunityID(string steamID)
        {
            Int64 authServer = Convert.ToInt64(steamID.Substring(8, 1));
            Int64 authID = Convert.ToInt64(steamID.Substring(10));
            return (76561197960265728 + (authID * 2) + authServer).ToString();
        }

        private string GetSteamID(Int64 communityID)
        {
            communityID = communityID - 76561197960265728;
            Int64 authServer = communityID % 2;
            communityID = communityID - authServer;
            Int64 authID = communityID / 2;
            return string.Format("STEAM_0:{0}:{1}", authServer, authID);
        }

        public string GetReadableSteamIdFromNs2Id(long steamIdNumber)
        {
            var result = string.Format("STEAM_0:{0}:{1}", steamIdNumber % 2, Math.Floor((double)(steamIdNumber / 2)));
            return result;
        }

        public long GetSteamCommunityProfileIdFromNs2Id(long ns2id)
        {
            var readableSteamId = GetReadableSteamIdFromNs2Id(ns2id);
            var result = GetSteamCommunityProfileIdFromReadableSteamId(readableSteamId);
            return result;
        }

        public long GetSteamCommunityProfileIdFromReadableSteamId(string steamId)
        {
            var parts = steamId.Substring(6).Split(':');
            var result = long.Parse(parts[1]) + long.Parse(parts[2]) * 2 + 76561197960265728L;
            return result;
        }
    }
}