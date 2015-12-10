using System.Text;
using System.Web.Configuration;

namespace ApiGeneratorApi.Models
{
    public class DataAccessGenerator
    {
        private EndpointSpec _apiSpecification;
        private string _modelType;
        public readonly string FilePath;

        public DataAccessGenerator(EndpointSpec apiSpecification, string modelType)
        {
            _apiSpecification = apiSpecification;
            _modelType = modelType;
            FilePath = string.Format("{0}/DataAccess/{1}", WebConfigurationManager.AppSettings["OutputFolder"], GetType());
        }

        public void Generate()
        {
            new FileWriter().WriteFile(FilePath, GenerateReader());
            new FileWriter().WriteFile(FilePath, GenerateWriter());
        }

        private string GenerateWriter()
        {
            var sb = new StringBuilder();
            sb.Append("using System.Data;");
            sb.Append("using System.Linq;");
            sb.Append("using Dapper;");
            sb.Append("using Giftango.Component.Utility;");
            sb.Append("namespace Giftango.Domain.Reader");
            sb.Append("{");
                sb.Append(string.Format("public class {0}Writer", _modelType));
                sb.Append("{");
                    sb.AppendFormat("public int Write({0} toWrite)", _modelType);
                    sb.Append("{");
                        sb.Append("using (var connection = ConnectionHelper.GetConnection())");
                        sb.Append("{");
                            sb.AppendFormat("return connection.Query<int>(\"[dbo].[Insert{0}]\"", _modelType).AppendLine("new { toWrite }, commandType: CommandType.StoredProcedure);");
                        sb.Append("}");
                    sb.Append("}");
                    sb.AppendFormat("public int WriteById(int id, {0} toWrite)", _modelType);
                    sb.Append("{");
                        sb.Append("using (var connection = ConnectionHelper.GetConnection())");
                        sb.Append("{");
                            sb.AppendFormat("return connection.Query<int}>(\"[dbo].[Get{0}]\"",_modelType).AppendLine("new {id, toWrite}, commandType: CommandType.StoredProcedure);");
                        sb.Append("}");
                    sb.Append("}");
                sb.Append("}");
            sb.Append("}");

            return sb.ToString();
        }

        private string GenerateReader()
        {
            var sb = new StringBuilder();
            sb.Append("using System.Data;");
            sb.Append("using System.Linq;");
            sb.Append("using Dapper;");
            sb.Append("using Giftango.Component.Utility;");
            sb.Append("namespace Giftango.Domain.Reader");
            sb.Append("{");
                sb.Append(string.Format("public class {0}Reader", _modelType));
                sb.Append("{");
                    sb.Append(string.Format("public List<{0}> GetAll()", _modelType));
                    sb.Append("{");
                        sb.Append("using (var connection = ConnectionHelper.GetConnection())");
                        sb.Append("{");
                            sb.Append(string.Format("return connection.Query<{0}>(\"[dbo].[GetAll{0}]\", commandType: CommandType.StoredProcedure);", _modelType));
                        sb.Append("}");
                    sb.Append("}");
                    sb.Append(string.Format("public {0} GetById(int id)", _modelType));
                    sb.Append("{");
                        sb.Append("using (var connection = ConnectionHelper.GetConnection())");
                        sb.Append("{");
                            sb.Append(string.Format("return connection.Query<{0}>(\"[dbo].[Get{0}]\", new {1}, commandType: CommandType.StoredProcedure);", _modelType, "{id}"));
                        sb.Append("}");
                    sb.Append("}");
                sb.Append("}");
            sb.Append("}");

            return sb.ToString();
        }

    }
}