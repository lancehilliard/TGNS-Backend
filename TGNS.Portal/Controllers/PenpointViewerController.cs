using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class PenpointViewerController : Controller
    {
        private readonly IPenpointEditDataGetter _penpointEditDataGetter;

        public PenpointViewerController()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Data"].ConnectionString;
            _penpointEditDataGetter = new PenpointEditDataGetter(connectionString);
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            var penpointViewModel = new PenpointViewModel();
            var editData = _penpointEditDataGetter.Get(id);
            penpointViewModel.EditData = editData;
            return View(penpointViewModel);
        }
	}
}