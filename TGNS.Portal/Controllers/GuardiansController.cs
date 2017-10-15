using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class GuardiansController : AdminController
    {
        private IGuardiansGetter _guardiansGetter;

        public GuardiansController()
        {
            _guardiansGetter = new GuardiansGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        public ActionResult Index()
        {
            var guardianDatas = _guardiansGetter.Get().Where(x => !string.IsNullOrWhiteSpace(x.BetterKnownAs));
            return View(guardianDatas);
        }
	}
}