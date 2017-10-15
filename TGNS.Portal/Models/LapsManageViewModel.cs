using System.Collections.Generic;
using TGNS.Core.Domain;

namespace TGNS.Portal.Models
{
    public class LapsManageViewModel
    {
        public IEnumerable<ITrack> Tracks { get; set; }
        public IEnumerable<LapDisplayModel> Laps { get; set; }
    }
}