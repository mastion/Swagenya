using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiGeneratorApi.Models
{
    public class WebApiGenerator
    {
        public readonly string FilePath;
        private readonly List<EndpointSpec> _apiSpecification;

        public WebApiGenerator(EndpointSpec apiSpecification)
        {
            _apiSpecification.Add(apiSpecification);
        }

        public WebApiGenerator(List<EndpointSpec> apiSpecification)
        {
            _apiSpecification = apiSpecification;
        }

        public void Generate()
        {
            foreach (
                var endpointList in _apiSpecification.GroupBy(x => x.Uri).Select(endpointSpec => endpointSpec.ToList()))
            {
                GenerateController(endpointList);
            }

            GenerateWebApiConfig();
        }

        private void GenerateController(IEnumerable<EndpointSpec> apiSpecification)
        {
            var builder = new StringBuilder();

            string modelType = _apiSpecification.First(x => x.Uri != null).Uri;

            builder.AppendLine(GenerateControllerHeader(modelType));

            foreach (var endpointSpec in apiSpecification)
            {
                builder.AppendLine(GenerateVerb(endpointSpec.HttpVerb, modelType));
            }

            builder.AppendLine(GenerateControllerFooter());
        }

        private static string GenerateControllerHeader(string modelType)
        {
            var builder = new StringBuilder();

            builder.AppendLine("using System.Web.Http;");
            builder.AppendLine("using Giftango.Domain.Models;");
            builder.AppendLine();
            builder.AppendLine("namespace Giftango.Domain.Controllers");
            builder.AppendLine("{");
            builder.AppendFormat("    public class {0}Controller : ApiController", modelType);
            builder.AppendLine();
            builder.AppendLine("    {");

            return builder.ToString();
        }

        private static string GenerateVerb(string verb, string modelType)
        {
            var builder = new StringBuilder();

            switch (verb)
            {
                case "get":
                    builder.AppendLine(GetEndpoint(modelType));
                    break;
                case "post":
                    builder.AppendLine(PostEndpoint(modelType));
                    break;
            }

            return builder.ToString();
        }

        private static string GetEndpoint(string modelType)
        {
            var builder = new StringBuilder();

            builder.AppendLine("        [HttpGet]");
            builder.AppendLine("        public IHttpActionResult Index()");
            builder.AppendLine("        {");
            builder.AppendLine("            return Ok();");
            builder.AppendLine("        }");

            return builder.ToString();
        }

        private static string PostEndpoint(string modelType)
        {
            //todo: Casing on object
            var builder = new StringBuilder();

            builder.AppendLine("        [HttpPost]");
            builder.AppendFormat("        public IHttpActionResult Generate({0} data)", modelType);
            builder.AppendLine();
            builder.AppendLine("        {");
            builder.AppendLine("            return Ok(data);");
            builder.AppendLine("        }");

            return builder.ToString();
        }

        private static string GenerateControllerFooter()
        {
            var builder = new StringBuilder();

            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateWebApiConfig()
        {
            var builder = new StringBuilder();

            builder.AppendLine(GenerateWebConfigHeader());

            foreach (var uri in _apiSpecification.GroupBy(x => x.Uri))
            {
                builder.AppendFormat("            config.Routes.MapHttpRoute(\"{0}\", ", uri);
                builder.Append("\"api/{controller}/{action}\");");
                builder.AppendLine();
            }

            builder.AppendLine(GenerateWebConfigFooter());
        }

        private static string GenerateWebConfigHeader()
        {
            var builder = new StringBuilder();

            builder.AppendLine("using System.Web.Http;");
            builder.AppendLine();
            builder.AppendLine("namespace ApiGeneratorApi");
            builder.AppendLine("{");
            builder.AppendLine("    public static class WebApiConfig");
            builder.AppendLine("    {");
            builder.AppendLine("        public static void Register(HttpConfiguration config)");
            builder.AppendLine("        {");
            builder.AppendLine("            // Web API configuration and services");
            builder.AppendLine();
            builder.AppendLine("            // Web API routes");
            builder.AppendLine("            config.MapHttpAttributeRoutes();");
            builder.AppendLine();

            return builder.ToString();
        }

        private static string GenerateWebConfigFooter()
        {
            var builder = new StringBuilder();

            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}