using System.Collections.Generic;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Core.Domain;

namespace TGNS.Portal.Models
{
    public class RecordingIndexViewModel
    {
        public IEnumerable<IGame> Ns2Games { get; set; }
        public IEnumerable<IGame> InfestedGames { get; set; }
        public IEnumerable<SelectListItem> RatingFilterOptions { get; set; }
        public IEnumerable<SelectListItem> CountFilterOptions { get; set; }
        public IEnumerable<SelectListItem> DurationFilterOptions { get; set; }
        public IEnumerable<VideoStatistics> VideoStatistics { get; set; }
        public double TotalGamesDurationInSeconds { get; set; }
        public long DistinctPlayerCount { get; set; }
    }
}