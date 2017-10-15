using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGNS.Core.Domain
{
    public interface IPreferredEntry
    {
        long PlayerId { get; }
        string PluginName { get; }
    }

    public class PreferredEntry : IPreferredEntry
    {
        public PreferredEntry(long playerId, string pluginName)
        {
            PlayerId = playerId;
            PluginName = pluginName;
        }

        public long PlayerId { get; private set; }
        public string PluginName { get; private set; }

        protected bool Equals(PreferredEntry other)
        {
            return PlayerId == other.PlayerId && string.Equals(PluginName, other.PluginName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PreferredEntry) obj);
        }
    }
}
