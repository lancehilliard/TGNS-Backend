using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TGNS.Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "PenPointId",
                "Penpoint/{id}",
                new { controller = "PenpointViewer", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"^.{22}$" }
            );

            routes.MapRoute(
                "PenPointHome",
                "Penpoint",
                new { controller = "Penpoint", action = "Edit" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
