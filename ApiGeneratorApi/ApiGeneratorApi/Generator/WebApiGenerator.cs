using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ApiGeneratorApi.Util;
using Raml.Parser.Expressions;

namespace ApiGeneratorApi.Models
{
    public class WebApiGenerator
    {
        private readonly List<EndpointSpec> _apiSpecification;
        private readonly IFileWriter _fileWriter;
        private readonly string _outputDirectory;
        private Resource _apiResource;
        private List<ResourceSpec> _resources;

        public WebApiGenerator(EndpointSpec apiSpecification) : this(new List<EndpointSpec> {apiSpecification})
        {
        }

        public WebApiGenerator(List<EndpointSpec> apiSpecification)
        {
            _fileWriter = new FileWriter();
            _apiSpecification = apiSpecification;
            _outputDirectory = String.Format(@"{0}", new FolderWriter().GetFolderName("WebApi"));
        }

        public WebApiGenerator(List<ResourceSpec> resourceSpecs)
        {
            _resources = resourceSpecs;
            _fileWriter = new FileWriter();
            _outputDirectory = String.Format(@"{0}", new FolderWriter().GetFolderName("WebApi"));
        }

        public void Generate()
        {
            foreach (var endpointList in _apiSpecification.GroupBy(x => x.Uri).ToList())
            {
                GenerateController(endpointList);
            }

            GenerateWebApiConfig();
        }

        public void Generate2()
        {
            foreach (var resource in _resources)
            {
                GenerateController(resource);
            }

            GenerateWebApiConfig();
        }

        private void GenerateController(ResourceSpec resource)
        {
            var builder = new StringBuilder();
            var modelType = resource.ResourceName;

            builder.AppendLine(GenerateControllerHeader(modelType));

            foreach (var endpointSpec in resource.Endpoints)
            {
                builder.AppendLine(GenerateVerb(endpointSpec.HttpVerb, modelType));
            }

            builder.AppendLine(GenerateControllerFooter());

            _fileWriter.WriteFile(
                String.Format(@"{0}/{1}Controller.cs", _outputDirectory,
                    modelType.First().ToString(CultureInfo.InvariantCulture).ToUpper() + modelType.Substring(1)),
                builder.ToString());
        }

        private void GenerateController(IEnumerable<EndpointSpec> apiSpecification)
        {
            var builder = new StringBuilder();

            var modelType = apiSpecification.First(x => x.Uri != null).Uri;
            modelType = modelType.First().ToString().ToUpper() + modelType.Substring(1);

            builder.AppendLine(GenerateControllerHeader(modelType));

            foreach (var endpointSpec in apiSpecification)
            {
                builder.AppendLine(GenerateVerb(endpointSpec.HttpVerb, modelType));
            }

            builder.AppendLine(GenerateControllerFooter());

            _fileWriter.WriteFile(
                String.Format(@"{0}/{1}Controller.cs", _outputDirectory,
                    modelType.First().ToString(CultureInfo.InvariantCulture).ToUpper() + modelType.Substring(1)),
                builder.ToString());
        }

        private static string GenerateControllerHeader(string modelType)
        {
            var builder = new StringBuilder();

            builder.AppendLine("using System.Web.Http;");
            builder.AppendLine("using Giftango.Domain.Models;");
            builder.AppendLine("using Giftango.Domain.Actions;");
            builder.AppendLine();
            builder.AppendFormat("namespace Giftango.Web.Admin.Pages.{0}",modelType).AppendLine();
            builder.AppendLine("{");
            builder.AppendFormat("    public class {0}Controller : ApiController",
                modelType);
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
            builder.AppendLine("        public IHttpActionResult Get(int? id)");
            builder.AppendLine("        {");
            builder.AppendFormat("            var bl = new {0}GetAction();",
                modelType.First().ToString(CultureInfo.InvariantCulture).ToUpper() + modelType.Substring(1));
            builder.AppendLine();
            builder.AppendLine("            return Ok(id == null? bl.GetAll(): bl.Get(id);");
            builder.AppendLine("        }");

            return builder.ToString();
        }

        private static string PostEndpoint(string modelType)
        {
            var builder = new StringBuilder();

            builder.AppendLine("        [HttpPost]");
            builder.AppendFormat("        public IHttpActionResult Post({0} data)",
                modelType.First().ToString(CultureInfo.InvariantCulture).ToUpper() + modelType.Substring(1));
            builder.AppendLine();
            builder.AppendLine("        {");
            builder.AppendFormat("          var tmpId = new {0}PostAction().Write(data);", modelType);
            builder.AppendLine();
            builder.AppendFormat("            return Ok(new {0}GetAction().Get(tempId));", modelType);
            builder.AppendLine();
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

            foreach (var spec in _apiSpecification.GroupBy(x => x.Uri))
            {
                builder.AppendFormat("            config.Routes.MapHttpRoute(\"{0}\", ", spec.First().Uri);
                builder.Append("\"api/{controller}/{action}\");");
                builder.AppendLine();
            }

            builder.AppendLine(GenerateWebConfigFooter());

            _fileWriter.WriteFile(String.Format(@"{0}\WebApiConfig.cs", _outputDirectory), builder.ToString());
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