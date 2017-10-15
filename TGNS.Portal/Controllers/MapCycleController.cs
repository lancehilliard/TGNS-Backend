using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class MapCycleController : Controller
    {
        private readonly MapCycleJsonGetter _mapCycleJsonGetter;

        public MapCycleController()
        {
            _mapCycleJsonGetter = new MapCycleJsonGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        public ActionResult Index()
        {
//            ViewBag.MapcycleJson = @"[
//  {
//    ""title"": ""apples"",
//    ""count"": [12000, 20000],
//    ""description"": {""text"": ""..."", ""sensitive"": false}
//  },
//  {
//    ""title"": ""oranges"",
//    ""count"": [17500, null],
//    ""description"": {""text"": ""..."", ""sensitive"": false}
//  }
//]";
            var mapCycleJson = _mapCycleJsonGetter.Get();
            //var mapDatas = GetMapDatas(mapCycleJson);
            return View(new MapCycleIndexViewModel {MapCycleJson = mapCycleJson});
        }

    }

    public class MapCycleIndexViewModel
    {
        public IEnumerable<MapData> MapDatas { get; set; }
        public string MapCycleJson { get; set; }
    }

    public class MapData
    {
        public MapData(string mapName, int min, int max)
        {
            MapName = mapName;
            Min = min;
            Max = max;
        }

        public string MapName { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }

    }
}