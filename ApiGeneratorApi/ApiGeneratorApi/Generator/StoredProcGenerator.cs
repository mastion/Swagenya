using System.Collections.Generic;
using System.Data;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class StoredProcGenerator
    {
        private EndpointSpec _endpointSpec;
        private string _modelType;
        //private List<string> _crudList;

        public StoredProcGenerator(EndpointSpec endpointSpec, string modelType)
        {
            _endpointSpec = endpointSpec;
            _modelType = modelType;
        }

        public void Generate()
        {
            string filePath;
            switch (_endpointSpec.HttpVerb.ToUpper())
            {
                case "GET":
                    filePath = string.Format("{0}/GetAll{1}.sql", new FolderWriter().GetFolderName("Sql"), _modelType);
                    new FileWriter().WriteFile(filePath, GenerateGetAll());
                    filePath = string.Format("{0}/Get{1}ById.sql", new FolderWriter().GetFolderName("Sql"), _modelType);
                    new FileWriter().WriteFile(filePath, GenerateGetById());
                    break;
                case "POST":
                    filePath = string.Format("{0}/Insert{1}.sql", new FolderWriter().GetFolderName("Sql"), _modelType);
                    new FileWriter().WriteFile(filePath, GenerateInsert());
                    break;
                case "PUT":
                    filePath = string.Format("{0}/Update{1}.sql", new FolderWriter().GetFolderName("Sql"), _modelType);
                    new FileWriter().WriteFile(filePath, GenerateUpdate());
                    break;
                case "DELETE":
                    filePath = string.Format("{0}/Delete{1}.sql", new FolderWriter().GetFolderName("Sql"), _modelType);
                    new FileWriter().WriteFile(filePath, GenerateDelete());
                    break;
            }
        }

        private string ModelParamList()
        {
            var sb = new StringBuilder();
            foreach (var responseSpec in _endpointSpec.Responses)
            {
                foreach (var payloadFieldSpec in responseSpec.Body)
                {
                    sb.AppendFormat("{0},", payloadFieldSpec.Name);
                }
            }
            return sb.ToString().Trim(',');
        }

        private string ModelParamListWithPrefix(char prefix)
        {
            var sb = new StringBuilder();
            foreach (var responseSpec in _endpointSpec.Responses)
            {
                foreach (var payloadFieldSpec in responseSpec.Body)
                {
                    sb.AppendFormat("{1}{0},", payloadFieldSpec.Name, prefix);
                }
            }
            return sb.ToString().Trim(',');
        }

        private string MapType(string type)
        {
            switch (type.ToUpper())
            {
                case "STRING":
                    return "VARCHAR(MAX)";
                case "DECIMAL":
                    return "MONEY";
                default:
                    return "";
            }
        }

        private string GenerateGetById()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[Get{0}] (", _modelType).AppendLine();
            sb.AppendFormat("   @Id INT").AppendLine();
            sb.AppendLine("   )");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();
            sb.AppendFormat("SELECT {0}",ModelParamList()).AppendLine();
            sb.AppendFormat("FROM dbo.{0}", _modelType).AppendLine();
            sb.AppendLine("WHERE Id = @Id");
            sb.AppendLine("GO").AppendLine();

            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[Get{0}]", _modelType).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateGetAll()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[GetAll{0}]", _modelType).AppendLine();
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();
            sb.AppendFormat("SELECT {0}", ModelParamList()).AppendLine();
            sb.AppendFormat("FROM dbo.{0}", _modelType).AppendLine();
            sb.AppendLine("GO").AppendLine();

            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[GetAll{0}]", _modelType).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateInsert()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[Insert{0}] (", _modelType).AppendLine();

            foreach (var responseSpec in _endpointSpec.Responses)
            {
                for (int i = 0; i < responseSpec.Body.Count; i++)
                {
                    sb.AppendFormat("@{0} {1}", responseSpec.Body[i].Name, MapType(responseSpec.Body[i].Type));
                    if (i + 1 < responseSpec.Body.Count)
                    {
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }
            }

            sb.AppendLine("   )");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();
            sb.AppendFormat("INSERT INTO dbo.{0} (", _modelType).AppendLine();
            sb.AppendFormat("{0}", ModelParamList()).AppendLine();
            sb.AppendLine(")");
            sb.AppendFormat("SELECT {0}", ModelParamListWithPrefix('@')).AppendLine();
            sb.AppendLine("GO").AppendLine();

            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[Insert{0}]", _modelType).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateUpdate()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[Update{0}] (", _modelType).AppendLine();
            sb.AppendLine("@Id INT,");
            foreach (var responseSpec in _endpointSpec.Responses)
            {
                for (int i = 0; i < responseSpec.Body.Count; i++)
                {
                    sb.AppendFormat("@{0} {1}", responseSpec.Body[i].Name, MapType(responseSpec.Body[i].Type));
                    if (i + 1 < responseSpec.Body.Count)
                    {
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }
            }

            sb.AppendLine("   )");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();

            sb.AppendFormat("UPDATE dbo.{0}", _modelType).AppendLine();
            sb.Append("SET");
            
            foreach (var responseSpec in _endpointSpec.Responses)
            {
                for (int i = 0; i < responseSpec.Body.Count; i++)
                {
                    sb.AppendFormat("{0} = @{0}", responseSpec.Body[i].Name);
                    if (i + 1 < responseSpec.Body.Count)
                    {
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }
            }
            sb.AppendLine("WHERE Id = @Id");
            sb.AppendLine("GO").AppendLine();
            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[Insert{0}]", _modelType).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateDelete()
        {
            return "";
        }

    }
}