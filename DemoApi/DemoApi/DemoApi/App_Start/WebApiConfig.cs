using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace DemoApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute("drink", "api/{controller}", new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("extra", "api/{controller}", new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
        }
    }
}

