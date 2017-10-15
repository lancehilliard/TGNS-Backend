using System.Collections.Generic;
using System.Web.Mvc;

namespace TGNS.Portal.Models
{
    public class PluginsViewModel
    {
        public string SelectedPluginName { get; set; }
        public IEnumerable<SelectListItem> PluginNames { get; set; }
        public string PluginConfigJson { get; set; }
    }
}