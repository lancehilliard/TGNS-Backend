using System.Collections;
using System.Collections.Generic;
using TGNS.Core.Data;

namespace TGNS.Portal.Models
{
    public class CaptainsEligibleViewModel
    {
        public IEnumerable<IPlayer> PlayersWhoHaveEarnedSilverCompetitorBadge { get; set; }
        public IEnumerable<IPlayer> PlayersWhoHaveEarnedBronzeWarriorBadge { get; set; }
    }
}