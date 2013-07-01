using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace saturdayclub.Scrawl
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Scrawl",
                url: "{action}",
                defaults: new { controller = "Scrawl", action = "Scrawl" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
        }
    }
}