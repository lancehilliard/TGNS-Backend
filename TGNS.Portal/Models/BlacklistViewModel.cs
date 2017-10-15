using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Core.Domain;

namespace TGNS.Portal.Models
{
    public class BlacklistViewModel
    {
        public BlacklistViewModel(IEnumerable<IBlacklistEntry> blacklistEntries)
        {
            BlacklistEntries = blacklistEntries;
        }

        public IEnumerable<IBlacklistEntry> BlacklistEntries { get; private set; }
    }
}