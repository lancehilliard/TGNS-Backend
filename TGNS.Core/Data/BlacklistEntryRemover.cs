using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBlacklistEntryRemover
    {
        void Remove(string realmName, IBlacklistEntry blacklistEntry);
    }

    public class BlacklistEntryRemover : DataAccessor, IBlacklistEntryRemover
    {
        private readonly IBlacklistEntriesGetter _blacklistEntriesGetter;
        private readonly IBlacklistEntriesSetter _blacklistEntriesSetter;

        public BlacklistEntryRemover(string connectionString, IBlacklistEntriesGetter blacklistEntriesGetter, IBlacklistEntriesSetter blacklistEntriesSetter) : base(connectionString)
        {
            _blacklistEntriesGetter = blacklistEntriesGetter;
            _blacklistEntriesSetter = blacklistEntriesSetter;
        }

        public void Remove(string realmName, IBlacklistEntry blacklistEntry)
        {
            var blacklistEntriesToPersist = _blacklistEntriesGetter.Get(realmName).Where(x=>!Equals(x, blacklistEntry)).ToList();;
            _blacklistEntriesSetter.Set("ns2", blacklistEntriesToPersist);
        }
    }
}