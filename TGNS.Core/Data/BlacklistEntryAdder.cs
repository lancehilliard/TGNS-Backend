using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IBlacklistEntryAdder
    {
        void Add(string realmName, IBlacklistEntry blacklistEntry);
    }

    public class BlacklistEntryAdder : DataAccessor, IBlacklistEntryAdder
    {
        private readonly IBlacklistEntriesGetter _blacklistEntriesGetter;
        private readonly IBlacklistEntriesSetter _blacklistEntriesSetter;

        public BlacklistEntryAdder(string connectionString, IBlacklistEntriesGetter blacklistEntriesGetter, IBlacklistEntriesSetter blacklistEntriesSetter) : base(connectionString)
        {
            _blacklistEntriesGetter = blacklistEntriesGetter;
            _blacklistEntriesSetter = blacklistEntriesSetter;
        }

        public void Add(string realmName, IBlacklistEntry blacklistEntry)
        {
            var blacklistEntriesToPersist = _blacklistEntriesGetter.Get(realmName).ToList();
            var entryAlreadyExists = blacklistEntriesToPersist.Contains(blacklistEntry);
            if (!entryAlreadyExists)
            {
                blacklistEntriesToPersist.Add(blacklistEntry);
            }
            _blacklistEntriesSetter.Set("ns2", blacklistEntriesToPersist);
        }
    }
}