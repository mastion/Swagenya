using System.Text;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Models
{
    public class DataAccessGenerator
    {
        private EndpointSpec _apiSpecification;
        private string _modelType;

        public DataAccessGenerator(EndpointSpec apiSpecification, string modelType)
        {
            _apiSpecification = apiSpecification;
            _modelType = modelType;

        }

        public void Generate()
        {
            var filePath = string.Format("{0}/{1}Reader.cs", new FolderWriter().GetFolderName("DataAccess"), _modelType);
            new FileWriter().WriteFile(filePath, GenerateReader());
             filePath = string.Format("{0}/{1}Writer.cs", new FolderWriter().GetFolderName("DataAccess"), _modelType);
            new FileWriter().WriteFile(filePath, GenerateWriter());
        }

        private string GenerateWriter()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Dapper;");
            sb.AppendLine("using Giftango.Component.Utility;");
            sb.AppendLine("namespace Giftango.Domain.Reader");
            sb.AppendLine("{");
                sb.AppendFormat("public class {0}Writer", _modelType);
                sb.AppendLine("{");
                    sb.AppendFormat("public int Write({0} toWrite)", _modelType);
                    sb.AppendLine("{");
                        sb.AppendLine("using (var connection = ConnectionHelper.GetConnection())");
                        sb.AppendLine("{");
                            sb.AppendFormat("return connection.Query<int>(\"[dbo].[Insert{0}]\"", _modelType).AppendLine("new { toWrite }, commandType: CommandType.StoredProcedure);");
                        sb.AppendLine("}");
                    sb.AppendLine("}");
                    sb.AppendFormat("public int WriteById(int id, {0} toWrite)", _modelType);
                    sb.AppendLine("{");
                        sb.AppendLine("using (var connection = ConnectionHelper.GetConnection())");
                        sb.AppendLine("{");
                            sb.AppendFormat("return connection.Query<int>(\"[dbo].[Get{0}]\"",_modelType).AppendLine("new {id, toWrite}, commandType: CommandType.StoredProcedure);");
                        sb.AppendLine("}");
                    sb.AppendLine("}");
                sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateReader()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Dapper;");
            sb.AppendLine("using Giftango.Component.Utility;");
            sb.AppendLine("namespace Giftango.Domain.Reader");
            sb.AppendLine("{");
                sb.AppendFormat("public class {0}Reader", _modelType).AppendLine();
                sb.AppendLine("{");
                    sb.AppendFormat("public List<{0}> GetAll()", _modelType).AppendLine();
                    sb.AppendLine("{");
                        sb.AppendLine("using (var connection = ConnectionHelper.GetConnection())");
                        sb.AppendLine("{");
                            sb.AppendFormat("return connection.Query<{0}>(\"[dbo].[GetAll{0}]\", commandType: CommandType.StoredProcedure);", _modelType).AppendLine();
                        sb.AppendLine("}");
                    sb.AppendLine("}");
                    sb.AppendFormat("public {0} GetById(int id)", _modelType).AppendLine();
                    sb.AppendLine("{");
                        sb.AppendLine("using (var connection = ConnectionHelper.GetConnection())");
                        sb.AppendLine("{");
                            sb.AppendFormat("return connection.Query<{0}>(\"[dbo].[Get{0}]\", new {1}, commandType: CommandType.StoredProcedure);", _modelType, "{id}");
                        sb.AppendLine("}");
                    sb.AppendLine("}");
                sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }

    }
}