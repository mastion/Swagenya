using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class DataAccessGenerator
    {
        private EndpointSpec _apiSpecification;
        private string _modelType;
        private IEnumerable<ResourceSpec> _resourceSpecs;

        public DataAccessGenerator(EndpointSpec apiSpecification, string modelType)
        {
            _apiSpecification = apiSpecification;
            _modelType = modelType;

        }

        public DataAccessGenerator(IEnumerable<ResourceSpec> resourceSpecs)
        {
            _resourceSpecs = resourceSpecs;
        }

        public void Generate()
        {
            foreach (var resourceSpec in _resourceSpecs)
            {
                var filePath = string.Format("{0}/{1}Reader.cs", new FolderWriter().GetFolderName("DataAccess"), resourceSpec.ResourceName);
                
                new FileWriter().WriteFile(filePath, GenerateReader(resourceSpec.ResourceName, resourceSpec.ResourceObjectType, resourceSpec.GetModel));
                filePath = string.Format("{0}/{1}Writer.cs", new FolderWriter().GetFolderName("DataAccess"), resourceSpec.ResourceName);
                new FileWriter().WriteFile(filePath, GenerateWriter(resourceSpec.ResourceName, resourceSpec.ResourceObjectType, resourceSpec.PostModel));
            }
        }

        private string GenerateWriter(string resourceName, string resourceObjectType, List<PayloadFieldSpec> modelObject)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Dapper;");
            sb.AppendLine("using Giftango.Component.Utility;");
            sb.AppendLine("namespace Giftango.Domain.Writer");
            sb.AppendLine("{");
                sb.AppendFormat("   public class {0}Writer", resourceName).AppendLine();
                sb.AppendLine("   {");
                sb.AppendFormat("      public int Write({0} toWrite)", resourceObjectType).AppendLine();
                sb.AppendLine("      {");
                sb.AppendLine("         using (var connection = ConnectionHelper.GetConnection())");
                sb.AppendLine("         {");
                sb.AppendFormat("            return connection.Query<int>(\"[dbo].[Insert{0}]\" new {{ {1} }}, commandType: CommandType.StoredProcedure);",resourceName, ModelParamList(modelObject));
                sb.AppendLine("         }");
                sb.AppendLine("      }").AppendLine();
                sb.AppendFormat("      public int WriteById(int id, {0} toWrite)", resourceObjectType).AppendLine();
                sb.AppendLine("      {");
                sb.AppendLine("         using (var connection = ConnectionHelper.GetConnection())");
                sb.AppendLine("         {");
                sb.AppendFormat("            return connection.Query<int>(\"[dbo].[Update{0}]\" new {{ id, {1}}}, commandType: CommandType.StoredProcedure);", resourceName, ModelParamList(modelObject));
                sb.AppendLine("         }");
                sb.AppendLine("      }");
                sb.AppendLine("   }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string ModelParamList(List<PayloadFieldSpec> fields)
        {
            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                sb.AppendFormat("toWrite.{0},", field.Name);
            }
            return sb.ToString().Trim(',');
        }

        private string GenerateReader(string resourceName, string resourceObjectType, List<PayloadFieldSpec> resource)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Dapper;");
            sb.AppendLine("using Giftango.Component.Utility;");
            sb.AppendLine("namespace Giftango.Domain.Reader");
            sb.AppendLine("{");
            sb.AppendFormat("   public class {0}Reader", resourceName).AppendLine();
            sb.AppendLine("   {");
            sb.AppendFormat("      public List<{0}> GetAll()", resourceObjectType).AppendLine();
            sb.AppendLine("      {");
            sb.AppendLine("         using (var connection = ConnectionHelper.GetConnection())");
            sb.AppendLine("         {");
            sb.AppendFormat("            return connection.Query<{0}>(\"[dbo].[GetAll{1}]\", commandType: CommandType.StoredProcedure);", resourceObjectType, resourceName).AppendLine();
            sb.AppendLine("         }");
            sb.AppendLine("      }");
            sb.AppendFormat("      public {0} GetById(int id)", resourceObjectType).AppendLine();
            sb.AppendLine("      {");
            sb.AppendLine("         using (var connection = ConnectionHelper.GetConnection())");
            sb.AppendLine("         {");
            sb.AppendFormat("            return connection.Query<{0}>(\"[dbo].[Get{1}]\", new {2}, commandType: CommandType.StoredProcedure);",resourceObjectType, resourceName, "{id}").AppendLine();
            sb.AppendLine("         }");
            sb.AppendLine("      }");
            sb.AppendLine("   }");
            sb.AppendLine("}");

            return sb.ToString();
        }

    }
}