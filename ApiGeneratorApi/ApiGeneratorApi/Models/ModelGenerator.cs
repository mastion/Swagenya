using System.IO;
using System.Text;
using System.Web.Configuration;

namespace ApiGeneratorApi.Models
{
    public class ModelGenerator
    {
        private readonly EndpointSpec _apiSpecification;
        private string _data;

        public readonly string FilePath;

        public ModelGenerator(EndpointSpec apiSpecification)
        {
            _apiSpecification = apiSpecification;

            FilePath = string.Format("{0}/Model/{1}", WebConfigurationManager.AppSettings["PFUserName"], GetType());
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
            using(var fs = new FileStream(FilePath, FileMode.Create))
            using(var sw = new StreamWriter(fs))
            sw.Write(_data);
        }

        private void CompileData()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("namespace Giftango.Domain.Models");
            sb.AppendFormat("   public class {0}", GetType()); sb.AppendLine();
            sb.AppendLine(" {");
            sb.AppendLine(WriteProperties());
            sb.AppendLine(" }");
            sb.AppendLine("}");

            _data = sb.ToString();
        }

        private string WriteProperties()
        {
            var sb = new StringBuilder();

            foreach (var myProperty in _apiSpecification.Request)
            {
                sb.AppendFormat(" public {0} {1} ", myProperty.Type, myProperty.Name);
                sb.AppendLine("{ get; set; }");
            }

            return sb.ToString();
        }

    }
}