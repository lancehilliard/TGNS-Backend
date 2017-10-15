using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Models
{
    public interface IFullSpecOptionsModel
    {
        IEnumerable<long> EnrolledPlayerIds { get; }
    }

    public class FullSpecOptionsModel : IFullSpecOptionsModel
    {
        public IEnumerable<long> EnrolledPlayerIds { get; private set; }

        public FullSpecOptionsModel(IEnumerable<long> enrolledPlayerIds)
        {
            EnrolledPlayerIds = enrolledPlayerIds;
        }
    }
}