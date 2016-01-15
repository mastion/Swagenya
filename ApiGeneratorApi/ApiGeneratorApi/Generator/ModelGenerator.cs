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
           // FilePath = string.Format("{0}/{1}.cs", new FolderWriter().GetFolderName("Model"), GetType());
        }

        public void Generate()
        {
            foreach (var resource in _resourceSpecs)
            {
                var resourceName = resource.ResourceName;
                foreach (var endPoint in resource.Endpoints)
                {
                    var methodName = ResourceSpec.ToTitleCase(endPoint.HttpVerb);
                    var modelName = String.Format("{0}{1}Model", methodName, resourceName);
                    if (endPoint.Request.Count > 0)
                    {
                        GenereateModelFile(modelName, GetFilePath(modelName), endPoint.Request);
                    }

                    foreach (var response in endPoint.Responses)
                    {
                        if (response.Body.Count > 0)
                        {
                            modelName = String.Format("{0}{1}{2}Model", methodName, response.StatusCode, resourceName);
                            GenereateModelFile(modelName, GetFilePath(modelName), response.Body);
                        }
                    }
                }
               
            }
        }

        private string GetFilePath(string modelName)
        {
            return string.Format("{0}{1}.cs", new FolderWriter().GetFolderName("Model"), modelName);
        }

        private void GenereateModelFile(string modelName, string filePath, List<PayloadFieldSpec> modelFields)
        {
            CompileData(modelName, modelFields);
            new FileWriter().WriteFile(filePath, _data);
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