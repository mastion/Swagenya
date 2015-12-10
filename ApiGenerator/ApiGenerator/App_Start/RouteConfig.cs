using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ApiGenerator
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "Cool end point", routeTemplate: "{action}");
            //    : "{controller}/{action}",
            //    defaults: new { controller = "ApiGenerator", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
