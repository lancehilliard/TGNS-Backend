using System.Collections.Generic;
using TGNS.Core.Data;

namespace TGNS.Portal.Models
{
    public class KarmaViewModel
    {
        public long? PlayerId { get; set; }
        public IEnumerable<IKarmaDelta> Deltas { get; set; }
        public string Bka { get; set; }
    }
}