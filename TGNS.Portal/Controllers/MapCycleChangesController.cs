using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Portal.Controllers
{
    public class MapCycleChangesController : AuthenticatedController
    {
        private readonly MapCycleJsonGetter _mapCycleJsonGetter;

        public MapCycleChangesController()
        {
            _mapCycleJsonGetter = new MapCycleJsonGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        //public JsonResult GetChangeableMapDatas()
        //{
        //    var mapCycleJson = _mapCycleJsonGetter.Get();
        //    var mapDatas = GetMapDatas(mapCycleJson);
        //    var mapNames = mapDatas.Select(x=>x.MapName);
        //    var ns2Maps = mapNames.Where(x=>x.StartsWith("ns2_"));
        //    var minOptions = new List<SelectListItem>();
        //    minOptions.Add(new SelectListItem {Text="Always in rotation", Value = "0"});
        //    minOptions.AddRange(Enumerable.Range(1, 24).Where(x => x <= 1 || x >= 16).Select(x => new SelectListItem {Text = $"When {x}+ are playing", Value = x.ToString()}));
        //    minOptions.Add(new SelectListItem { Text = "Only when nominated", Value = "99" });

        //    return Json(new Dictionary<string, object> { { "success", attemptResult.Success }, { "msg", attemptResult.ErrorMessage } }, JsonRequestBehavior.AllowGet);
        //}

        private IEnumerable<MapData> GetMapDatas(string mapCycleJson)
        {
            var result = new List<MapData>();
            dynamic mapCycleData = JsonConvert.DeserializeObject(mapCycleJson);
            dynamic maps = mapCycleData.maps;
            foreach (var m in maps)
            {
                string mapName;
                var min = 0;
                var max = 99;
                if (m is string)
                {
                    mapName = m;
                }
                else
                {
                    mapName = m.map;
                    min = m.min;
                    max = m.max;
                }
                Console.WriteLine(mapName);
                result.Add(new MapData(mapName, min, max));
            }
            return result;
        }

    }
}