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
        private readonly FileWriter _fileWriter;
        private readonly string _modelType;
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
            _fileWriter = new FileWriter();
        }

        public void Generate()
        {
            CompileData();
            _fileWriter.WriteFile(FilePath, _data);
        }

        private void CompileData()
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Giftango.Domain.Models;");
            sb.AppendLine("using Giftango.Domain.Writer;");
            sb.AppendLine("using Giftango.Domain.Reader;");
            sb.AppendLine();
            sb.AppendLine("namespace Giftango.Domain.Actions");
            sb.AppendLine("{"); //start namespace

            //Interface
            sb.AppendLine(CompileObject(false));
            //Concrete implementation
            sb.AppendLine(CompileObject(true));

            sb.AppendLine("}"); //end namespace

            _data = sb.ToString();
        }

        private string CompileObject(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("     public {0}{1}{2}Action", isConcrete ? "class " : "interface I", _modelType,
                _actionType);
            sb.AppendLine(isConcrete ? string.Format(" : I{0}{1}Action", _modelType, _actionType) : "");
                //Inherit from interface if isConcrete
            sb.AppendLine("     {");

            if (isConcrete)
                sb.AppendLine(CompileProperties(_actionType));

            sb.AppendLine();

            if (isConcrete)
                sb.AppendLine(CompileConstrutors(_actionType));

            sb.Append(CompileActionFunction(isConcrete));

            sb.AppendLine("     }");
            return sb.ToString();
        }

        private string CompileActionFunction(bool isConcrete)
        {
            var sb = new StringBuilder();

            switch (_actionType.ToUpper())
            {
                case "GET":
                    sb.Append(CompileReadAllFunction(isConcrete));
                    sb.Append(CompileReadFunction(isConcrete));
                    break;
                case "POST":
                    sb.Append(CompileWriteFunction(isConcrete));
                    break;
                case "UPDATE":
                    sb.Append(CompileUpdateFunction(isConcrete));
                    break;
            }

            return sb.ToString();
        }

        private string CompileProperties(string actionType)
        {
            var sb = new StringBuilder();

            switch (actionType.ToLower())
            {
                case "post":
                    sb.AppendFormat("         private readonly {0}Writer _writer;", _modelType).AppendLine();
                    break;
                case "put":
                    sb.AppendFormat("         private readonly {0}Reader _reader;", _modelType).AppendLine();
                    sb.AppendFormat("         private readonly {0}Writer _writer;", _modelType).AppendLine();
                    break;
                case "get":
                    sb.AppendFormat("         private readonly {0}Reader _reader;", _modelType).AppendLine();
                    break;
            }


            return sb.ToString();
        }

        private string CompileConstrutors(string actionType)
        {
            var sb = new StringBuilder();

            switch (actionType.ToLower())
            {
                case "post":
                    sb.AppendFormat("         public {0}{1}Action() : this(new {0}Writer())", _modelType, _actionType)
                        .AppendLine();
                    sb.AppendLine("         { }").AppendLine();
                    sb.AppendFormat("         public {0}{1}Action ({0}Writer writer)", _modelType, _actionType)
                        .AppendLine();
                    sb.AppendLine("         {");
                    sb.AppendLine("           _writer = writer;");
                    sb.AppendLine("         }");
                    break;
                case "put":
                    sb.AppendFormat("         public {0}{1}Action() : this(new {0}Reader(), new {0}Writer())",
                        _modelType, _actionType).AppendLine();
                    sb.AppendLine("         { }").AppendLine();
                    sb.AppendFormat("         public {0}{1}Action ({0}Reader reader, {0}Writer writer)", _modelType,
                        _actionType).AppendLine();
                    sb.AppendLine("         {");
                    sb.AppendLine("           _reader = reader;");
                    sb.AppendLine("           _writer = writer;");
                    sb.AppendLine("         }");
                    break;
                case "get":
                    sb.AppendFormat("         public {0}{1}Action() : this(new {0}Reader())", _modelType, _actionType)
                        .AppendLine();
                    sb.AppendLine("         { }").AppendLine();
                    sb.AppendFormat("         public {0}{1}Action ({0}Reader reader)", _modelType, _actionType)
                        .AppendLine();
                    sb.AppendLine("         {");
                    sb.AppendLine("           _reader = reader;");
                    sb.AppendLine("         }");
                    break;
            }


            return sb.ToString();
        }

        private string CompileWriteFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}int Write({1} data)", isConcrete ? "public " : "", _modelType);

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

        private string CompileUpdateFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}int WriteById(int Id {1} data)", isConcrete ? "public " : "", _modelType);

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

        private string CompileReadFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}{1} Get(int Id)", isConcrete ? "public " : "", _modelType);
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

        private string CompileReadAllFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}List<{1}> GetAll()", isConcrete ? "public " : "", _modelType);
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