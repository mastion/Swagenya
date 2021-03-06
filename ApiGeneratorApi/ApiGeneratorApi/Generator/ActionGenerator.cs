using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class ActionGenerator
    {
        //private readonly EndpointSpec _apiSpecification;

        public readonly string FilePath;
        private readonly string _actionType;
        private readonly FileWriter _fileWriter = new FileWriter();
        private readonly string _modelType;
        private IEnumerable<ResourceSpec> _resourceSpecs;
        private string _data;

        public ActionGenerator(EndpointSpec apiSpecification, string modelType)
        {
            //_apiSpecification = apiSpecification;
            _modelType = modelType;

            //Formatting
            _actionType = apiSpecification.HttpVerb.ToLower();
            _actionType = _actionType[0].ToString(CultureInfo.InvariantCulture).ToUpper() + _actionType.Substring(1);

            FilePath = string.Format("{0}/{1}Action.cs",
                new FolderWriter().GetFolderName(string.Format("{0}Actions", _modelType)), _actionType);
        }

        public ActionGenerator(IEnumerable<ResourceSpec> resourceSpecs)
        {
            _resourceSpecs = resourceSpecs;
        }

        public void Generate()
        {
            foreach (var resourceSpec in _resourceSpecs)
            {
                var resourceName = resourceSpec.ResourceName;
                foreach (var endPoint in resourceSpec.Endpoints)
                {
                    CompileData(endPoint.HttpVerb, resourceName);
                }
            }
        }

        private void CompileData(string httpVerb, string resourceName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Giftango.Domain.Models;");
            sb.AppendLine("using Giftango.Domain.Writer;");
            sb.AppendLine("using Giftango.Domain.Reader;");
            sb.AppendLine();
            sb.AppendLine("namespace Giftango.Domain.Actions");
            sb.AppendLine("{"); //start namespace

            var actionName = String.Format("{0}{1}Action", httpVerb, resourceName);

            //Interface
            sb.AppendLine(CompileObject(false, httpVerb, resourceName));
            //Concrete implementation
            sb.AppendLine(CompileObject(true, httpVerb, resourceName));

            sb.AppendLine("}"); //end namespace

            _data = sb.ToString();

            var filePath = string.Format("{0}/{1}.cs",
              new FolderWriter().GetFolderName(string.Format("{0}Actions", resourceName)), actionName);
            _fileWriter.WriteFile(filePath, _data);
        }

        private string CompileObject(bool isConcrete, string httpVerb, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("     public {0}{1}{2}Action", isConcrete ? "class " : "interface I", httpVerb, resourceName);
            sb.AppendLine(isConcrete ? string.Format(" : I{0}{1}Action", httpVerb, resourceName) : ""); //Inherit from interface if isConcrete
            sb.AppendLine("     {");
            sb.AppendLine(CompileProperties(isConcrete, resourceName));
            sb.AppendLine();

            if (isConcrete)
                sb.AppendLine(CompileConstrutors(httpVerb, resourceName));

            sb.Append(CompileActionFunction(isConcrete, httpVerb, resourceName));

            sb.AppendLine("     }");
            return sb.ToString();
        }

        private string CompileActionFunction(bool isConcrete, string httpVerb, string resourceName)
        {
            var sb = new StringBuilder();

            switch (httpVerb.ToUpper())
            {
                case "GET":
                    sb.Append(CompileReadAllFunction(isConcrete, resourceName));
                    sb.Append(CompileReadFunction(isConcrete, resourceName));
                    break;
                case "POST":
                    sb.Append(CompileWriteFunction(isConcrete, resourceName));
                    break;
                case "UPDATE":
                    sb.Append(CompileUpdateFunction(isConcrete, resourceName));
                    break;
            }

            return sb.ToString();
        }

        private string CompileProperties(bool isConcrete, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}readonly I{1}Reader _reader;", isConcrete ? "private " : "", resourceName).AppendLine();
            sb.AppendFormat("         {0}readonly I{1}Writer _writer;", isConcrete ? "private " : "", resourceName);
            return sb.ToString();
        }

        private string CompileConstrutors(string httpVerb, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         public {0}{1}Action() : this(new {0}Reader(), new {0}Writer())", httpVerb, resourceName).AppendLine();
            sb.AppendLine("         { }").AppendLine();
            sb.AppendFormat("         public {0}{1}Action (I{0}Reader reader, I{0}Writer writer)", httpVerb, resourceName).AppendLine();
            sb.AppendLine("         {");
            sb.AppendLine("           _reader = reader;");
            sb.AppendLine("           _writer = writer;");
            sb.AppendLine("         }");
            return sb.ToString();
        }

        private string CompileWriteFunction(bool isConcrete, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}int Write({1} data)", isConcrete ? "public " : "", resourceName);
            
            if (isConcrete)
            {
                sb.AppendLine();
                sb.AppendLine("         {");
                sb.AppendLine("             return _writer.Write(data);");
                sb.AppendLine("         }");
            }
            else
            {
                sb.AppendLine(";");
            }
            return sb.ToString();
        }

        private string CompileUpdateFunction(bool isConcrete, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}int WriteById(int Id {1} data)", isConcrete ? "public " : "", resourceName);
            
            if (isConcrete)
            {
                sb.AppendLine();
                sb.AppendLine("         {");
                sb.AppendLine("             return _writer.Write(Id, data);");
                sb.AppendLine("         }");
            }
            else
            {
                sb.AppendLine(";");
            }
            return sb.ToString();
        }

        private string CompileReadFunction(bool isConcrete, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}{1} Get(int Id)", isConcrete ? "public " : "", resourceName);
            if (isConcrete)
            {
                sb.AppendLine();
                sb.AppendLine("         {");
                sb.AppendLine("             return _reader.GetById(Id);");
                sb.AppendLine("         }");
            }
            else
            {
                sb.AppendLine(";");
            }
            return sb.ToString();
        }

        private string CompileReadAllFunction(bool isConcrete, string resourceName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}List<{1}> GetAll()", isConcrete ? "public " : "", resourceName);
            if (isConcrete)
            {
                sb.AppendLine();
                sb.AppendLine("         {");
                sb.AppendLine("             return _reader.GetAll();");
                sb.AppendLine("         }");
            }
            else
            {
                sb.AppendLine(";");
            }
            return sb.ToString();
        }
    }
}