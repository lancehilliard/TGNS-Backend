using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Models
{
    public class FullSpecOptionsViewModel
    {
        public IEnumerable<int> EnrolledPlayerIds { get; private set; }

        public FullSpecOptionsViewModel(IEnumerable<int> enrolledPlayerIds)
        {
            EnrolledPlayerIds = enrolledPlayerIds;
        }
    }
}