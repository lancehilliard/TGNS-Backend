using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IPreferredEntryRemover
    {
        void Remove(string realmName, IPreferredEntry preferredEntry);
    }

    public class PreferredEntryRemover : DataAccessor, IPreferredEntryRemover
    {
        private readonly IPreferredEntriesGetter _preferredEntriesGetter;
        private readonly IPreferredEntriesSetter _preferredEntriesSetter;

        public PreferredEntryRemover(string connectionString, IPreferredEntriesGetter preferredEntriesGetter, IPreferredEntriesSetter preferredEntriesSetter) : base(connectionString)
        {
            _preferredEntriesGetter = preferredEntriesGetter;
            _preferredEntriesSetter = preferredEntriesSetter;
        }

        public void Remove(string realmName, IPreferredEntry preferredEntry)
        {
            var preferredEntriesToPersist = _preferredEntriesGetter.Get(realmName).Where(x=>!Equals(x, preferredEntry)).ToList();;
            _preferredEntriesSetter.Set("ns2", preferredEntriesToPersist);
        }
    }
}
