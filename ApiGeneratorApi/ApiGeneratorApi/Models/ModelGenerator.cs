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

            FilePath = string.Format("{0}/Model/{1}", WebConfigurationManager.AppSettings["OutputFolder"], GetType());
        }

        public new string GetType()
        {
            return _apiSpecification.Uri;
        }

        public void Generate()
        {
            CompileData();
            FileWriter.WriteFile(FilePath,_data);
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


            foreach (var myProperty in _apiSpecification.Responses)
            {
                foreach (var myBody in myProperty.Body)
                {
                    sb.AppendFormat(" public {0} {1} ", myBody.Type, myBody.Name);
                    sb.AppendLine("{ get; set; }");
                }                
            }


            return sb.ToString();
        }

    }
}