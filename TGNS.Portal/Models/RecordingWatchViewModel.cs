using System.Collections.Generic;
using System.Web.Mvc;

namespace TGNS.Portal.Models
{
    public class RecordingWatchViewModel
    {
        public IEnumerable<SelectListItem> CameraOptions { get; set; }  
    }
}