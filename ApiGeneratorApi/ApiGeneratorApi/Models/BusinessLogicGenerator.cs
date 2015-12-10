using System.IO;
using System.Text;
using System.Web.Configuration;

namespace ApiGeneratorApi.Models
{
    public class BusinessLogicGenerator
    {
        private readonly EndpointSpec _apiSpecification;
        private string _data;

        public readonly string FilePath;
        private readonly string ClassSuffix = "BusinessLogic";

        public BusinessLogicGenerator(EndpointSpec apiSpecification)
        {
            _apiSpecification = apiSpecification;

            FilePath = string.Format("{0}/BusinessLogic/{1}", WebConfigurationManager.AppSettings["PFUserName"], GetType());
        }

        public new string GetType()
        {
            return _apiSpecification.Uri;
        }

        public void Generate()
        {
            CompileData();
            WriteFile();
        }

        private void WriteFile()
        {
            using (var fs = new FileStream(FilePath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                sw.Write(_data);
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
            sb.AppendFormat("   public {0}{1}{2}", isConcrete? "class " : "interface i", GetType(), ClassSuffix);
            sb.AppendLine(isConcrete? string.Format(" : I{0}{1}", GetType(), ClassSuffix): ""); //Inherit from interface if isConcrete
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine(CompileProperties(isConcrete));
            sb.AppendLine();
            
            if (isConcrete)
            sb.AppendLine(CompileConstrutors());

            sb.AppendLine(CompileReadAllFunction(isConcrete));
            sb.AppendLine(CompileReadFunction(isConcrete));
            sb.AppendLine(CompileWriteFunction(isConcrete));

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string CompileProperties(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" {0}readonly I{1}Reader _reader;", isConcrete ? "private " : "", GetType());
            sb.AppendFormat(" {0}readonly I{1}Writer _writer;", isConcrete ? "private " : "", GetType());
            return sb.ToString();
        }

        private string CompileConstrutors()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("       public {0}{1}() : this(new {0}Reader(), new {0}Writer())", GetType(), ClassSuffix);
            sb.AppendLine("{}");
            sb.AppendFormat("       public {0}{1}() (I{0}Reader reader, I{0}Writer writer)", GetType(), ClassSuffix);
            sb.AppendLine(" {");
            sb.AppendLine("         _reader = reader;");
            sb.AppendLine("         _writer = writer;");
            sb.AppendLine("     }");
            return sb.ToString();
        }

        private string CompileWriteFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("       {0}int Write({1} data)", isConcrete? "public " : "", GetType());
            sb.AppendLine("{");
            sb.AppendLine("             return _writer.Write(data);");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string CompileReadFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("       {0}{1} Get(int Id)", isConcrete ? "public " : "", GetType());
            sb.AppendLine("{");
            sb.AppendLine("             return _reder.Get(Id);");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string CompileReadAllFunction(bool isConcrete)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("       {0}List<{1}> GetAll()", isConcrete ? "public " : "", GetType());
            sb.AppendLine("{");
            sb.AppendLine("             return _reder.GetAll();");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}