using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class ModelGenerator
    {
        private readonly EndpointSpec _apiSpecification;
        private string _data;

        public readonly string FilePath;
        private IEnumerable<ResourceSpec> _resourceSpecs;

        public ModelGenerator(EndpointSpec apiSpecification)
        {
            _apiSpecification = apiSpecification;

            FilePath = string.Format("{0}/{1}.cs", new FolderWriter().GetFolderName("Model"), GetType());
        }

        public ModelGenerator(IEnumerable<ResourceSpec> resourceSpecs)
        {
            _resourceSpecs = resourceSpecs;
        }

        public new string GetType()
        {
            return string.Format("{0}{1}",_apiSpecification.Uri[0].ToString(CultureInfo.InvariantCulture).ToUpper(), _apiSpecification.Uri.Substring(1));
        }

        public void Generate()
        {
            foreach (var resource in _resourceSpecs)
            {
                var resourceName = resource.ResourceName;
                foreach (var endPoint in resource.Endpoints)
                {
                    var methodName = endPoint.HttpVerb;
                    CompileData(String.Format("{0}{1}Model", methodName, resourceName), endPoint.Request);
                    new FileWriter().WriteFile(FilePath, _data);
                }
               
            }
        }

        private void CompileData(string modelName, List<PayloadFieldSpec> modelFields)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("namespace Giftango.Domain.Models");
            sb.AppendLine("{");
            sb.AppendFormat("   public class {0}", modelName).AppendLine();
            sb.AppendLine("     {");
            sb.Append(WriteProperties(modelFields));
            sb.AppendLine("     }");
            sb.AppendLine(" }");

            _data = sb.ToString();
        }

        private string WriteProperties(List<PayloadFieldSpec> modelFields)
        {
            var sb = new StringBuilder();

            foreach (var field in modelFields)
            {
                sb.AppendFormat("       public {0} {1} ", field.Type, field.Name);
                sb.AppendLine("{ get; set; }");
            }
            return sb.ToString();
        }
    }
}