using System.Collections.Generic;
using System.Web.Mvc;

namespace TGNS.Portal.Models
{
    public class RecordingsAddViewModel
    {
        public IEnumerable<SelectListItem> GamesOptions { get; set; }  
        public string ErrorMessage { get; set; }
        public string DefaultVideoIdentifier { get; set; }
        public string DefaultGameOptionValue { get; set; }
    }
}