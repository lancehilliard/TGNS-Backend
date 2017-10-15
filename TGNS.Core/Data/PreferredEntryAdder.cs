using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IPreferredEntryAdder
    {
        void Add(string realmName, IPreferredEntry preferredEntry);
    }

    public class PreferredEntryAdder : DataAccessor, IPreferredEntryAdder
    {
        private readonly IPreferredEntriesGetter _preferredEntriesGetter;
        private readonly IPreferredEntriesSetter _preferredEntriesSetter;

        public PreferredEntryAdder(string connectionString, IPreferredEntriesGetter preferredEntriesGetter, IPreferredEntriesSetter preferredEntriesSetter) : base(connectionString)
        {
            _preferredEntriesGetter = preferredEntriesGetter;
            _preferredEntriesSetter = preferredEntriesSetter;
        }

        public void Add(string realmName, IPreferredEntry preferredEntry)
        {
            var preferredEntriesToPersist = _preferredEntriesGetter.Get(realmName).ToList();
            var entryAlreadyExists = preferredEntriesToPersist.Contains(preferredEntry);
            if (!entryAlreadyExists)
            {
                preferredEntriesToPersist.Add(preferredEntry);
            }
            _preferredEntriesSetter.Set("ns2", preferredEntriesToPersist);
        }
    }
}
