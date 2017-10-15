using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Core.Domain;

namespace TGNS.Portal.Models
{
    public class PreferredViewModel
    {
        public PreferredViewModel(IEnumerable<IPreferredEntry> preferredEntries)
        {
            PreferredEntries = preferredEntries;
        }

        public IEnumerable<IPreferredEntry> PreferredEntries { get; private set; }
    }
}