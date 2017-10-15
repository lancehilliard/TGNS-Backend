using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Controllers
{
    public class SpecBetsController : Controller
    {
        private readonly ISpecBetsGetter _specBetsGetter;

        public SpecBetsController()
        {
            _specBetsGetter = new SpecBetsGetter(new BkaDataParser());
        }

        // GET: SpecBets
        public ActionResult Index()
        {
            var specBetsTotals = _specBetsGetter.Get();
            return View(specBetsTotals);
        }
    }
}