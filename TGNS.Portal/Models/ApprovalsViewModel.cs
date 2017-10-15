using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Core.Domain;

namespace TGNS.Portal.Models
{
    public class ApprovalsViewModel
    {
        public IEnumerable<IApproval> Approvals { get; private set; }

        public ApprovalsViewModel(IEnumerable<IApproval> approvals)
        {
            Approvals = approvals;
        }
    }
}