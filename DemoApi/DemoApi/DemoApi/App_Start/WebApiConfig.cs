using System.Web.Http;

namespace ApiGeneratorApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute("drink", "api/{controller}/{action}");
            config.Routes.MapHttpRoute("extra", "api/{controller}/{action}");
        }
    }
}

