using System.IO;
using System.Text;
using System.Web.Configuration;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Models
{
    public class BusinessLogicGenerator
    {
        private readonly EndpointSpec _apiSpecification;
        private string _data;

        public readonly string FilePath;
        private readonly string _modelType;
        private readonly FileWriter _fileWriter;
        private const string ClassSuffix = "BusinessLogic";

        public BusinessLogicGenerator(EndpointSpec apiSpecification, string modelType)
        {
            _apiSpecification = apiSpecification;
            _modelType = modelType;
            FilePath = string.Format("{0}/{1}Action.cs", new FolderWriter().GetFolderName(ClassSuffix), _modelType);
            _fileWriter = new FileWriter();
        }

        public void Generate()
        {
            CompileData();
            new FileWriter().WriteFile(FilePath, _data);
        }

        private void CompileData()
        {
            var sb = new StringBuilder();

            sb.AppendLine("using namespace Giftango.Domain.Models");
            sb.AppendLine("using namespace Giftango.Domain.Readers");
            sb.AppendLine("using namespace Giftango.Domain.Writers");
            sb.AppendLine();
            sb.AppendFormat("namespace Giftango.Domain.{0}", ClassSuffix); sb.AppendLine();
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
            sb.AppendFormat("     public {0}{1}{2}", isConcrete? "class " : "interface I", _modelType, ClassSuffix);
            sb.AppendLine(isConcrete? string.Format(" : I{0}{1}", _modelType, ClassSuffix): ""); //Inherit from interface if isConcrete
            sb.AppendLine("     {");
            sb.AppendLine(CompileProperties(isConcrete));
            sb.AppendLine();
            
            if (isConcrete)
            sb.AppendLine(CompileConstrutors());

            sb.AppendLine(CompileReadAllFunction(isConcrete));
            sb.AppendLine(CompileReadFunction(isConcrete));
            sb.AppendLine(CompileWriteFunction(isConcrete));

            sb.AppendLine("     }");
            return sb.ToString();
        }

        private string CompileProperties(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}readonly I{1}Reader _reader;", isConcrete ? "private " : "", _modelType);
            sb.AppendLine();
            sb.AppendFormat("         {0}readonly I{1}Writer _writer;", isConcrete ? "private " : "", _modelType);
            return sb.ToString();
        }

        private string CompileConstrutors()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         public {0}{1}() : this(new {0}Reader(), new {0}Writer())", _modelType, ClassSuffix);
            sb.AppendLine();
            sb.AppendLine("         { }");
            sb.AppendFormat("         public {0}{1}() (I{0}Reader reader, I{0}Writer writer)", _modelType, ClassSuffix);
            sb.AppendLine();
            sb.AppendLine("         {");
            sb.AppendLine("           _reader = reader;");
            sb.AppendLine("           _writer = writer;");
            sb.AppendLine("         }");
            return sb.ToString();
        }

        private string CompileWriteFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}int Write({1} data)", isConcrete? "public " : "", _modelType);
            sb.AppendLine();
            sb.AppendLine("         {");
            sb.AppendLine("             return _writer.Write(data);");
            sb.AppendLine("         }");
            return sb.ToString();
        }

        private string CompileReadFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}{1} Get(int Id)", isConcrete ? "public " : "", _modelType);
            sb.AppendLine();
            sb.AppendLine("         {");
            sb.AppendLine("             return _reder.Get(Id);");
            sb.AppendLine("         }");
            return sb.ToString();
        }

        private string CompileReadAllFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("         {0}List<{1}> GetAll()", isConcrete ? "public " : "", _modelType);
            sb.AppendLine();
            sb.AppendLine("         {");
            sb.AppendLine("             return _reder.GetAll();");
            sb.AppendLine("         }");
            return sb.ToString();
        }
    }
}