using System.IO;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class ActionGenerator
    {
        //private readonly EndpointSpec _apiSpecification;
        private string _data;

        public readonly string FilePath;
        private readonly string _modelType;
        private readonly FileWriter _fileWriter;
        private readonly string _actionType;

        public ActionGenerator(EndpointSpec apiSpecification, string modelType)
        {
            //_apiSpecification = apiSpecification;
            _modelType = modelType;
            
            //Formatting
            _actionType = apiSpecification.HttpVerb.ToLower();
            _actionType = _actionType[0].ToString().ToUpper() + _actionType.Substring(1);
            
            FilePath = string.Format("{0}/{1}Action.cs", new FolderWriter().GetFolderName("Actions"), _actionType);
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

            sb.AppendLine("using namespace Giftango.Domain.Models");
            sb.AppendLine("using namespace Giftango.Domain.Readers");
            sb.AppendLine("using namespace Giftango.Domain.Writers");
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
            sb.AppendFormat("     public {0}{1}{2}Action", isConcrete ? "class " : "interface I", _modelType, _actionType);
            sb.AppendLine(isConcrete ? string.Format(" : I{0}{1}Action", _modelType, _actionType) : ""); //Inherit from interface if isConcrete
            sb.AppendLine("     {");
            sb.AppendLine(CompileProperties(isConcrete));
            sb.AppendLine();

            if (isConcrete)
                sb.AppendLine(CompileConstrutors());

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

        private string CompileProperties(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}readonly I{1}Reader _reader;", isConcrete ? "private " : "", _modelType).AppendLine();
            sb.AppendFormat("         {0}readonly I{1}Writer _writer;", isConcrete ? "private " : "", _modelType);
            return sb.ToString();
        }

        private string CompileConstrutors()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         public {0}{1}Action() : this(new {0}Reader(), new {0}Writer())", _modelType, _actionType).AppendLine();
            sb.AppendLine("         { }").AppendLine();
            sb.AppendFormat("         public {0}{1}Action() (I{0}Reader reader, I{0}Writer writer)", _modelType, _actionType).AppendLine();
            sb.AppendLine("         {");
            sb.AppendLine("           _reader = reader;");
            sb.AppendLine("           _writer = writer;");
            sb.AppendLine("         }");
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
                sb.AppendLine("             return _reder.Get(Id);");
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
                sb.AppendLine("             return _reder.GetAll();");
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