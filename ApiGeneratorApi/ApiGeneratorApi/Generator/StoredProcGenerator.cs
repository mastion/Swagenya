using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class StoredProcGenerator
    {
        private IEnumerable<ResourceSpec> _resources;
        //private List<string> _crudList;

        public StoredProcGenerator(IEnumerable<ResourceSpec> resourceSpecs)
        {
            _resources = resourceSpecs;
        }

        public void Generate()
        {
            string filePath;
            foreach (var resourceSpec in _resources)
            {
                foreach (var endpointSpec in _resources.SelectMany(r => r.Endpoints))
                {
                    switch (endpointSpec.HttpVerb.ToUpper())
                    {
                        case "GET":
                            filePath = string.Format("{0}/GetAll{1}.sql", new FolderWriter().GetFolderName("Sql"), resourceSpec.ResourceName);
                            new FileWriter().WriteFile(filePath, GenerateGetAll(resourceSpec.ResourceName, resourceSpec.GetModel));
                            filePath = string.Format("{0}/Get{1}ById.sql", new FolderWriter().GetFolderName("Sql"), resourceSpec.ResourceName);
                            new FileWriter().WriteFile(filePath, GenerateGetById(resourceSpec.ResourceName, resourceSpec.GetModel));
                            break;
                        case "POST":
                            filePath = string.Format("{0}/Insert{1}.sql", new FolderWriter().GetFolderName("Sql"), resourceSpec.ResourceName);
                            new FileWriter().WriteFile(filePath, GenerateInsert(resourceSpec.ResourceName, resourceSpec.PostModel));
                            break;
                        case "PUT":
                            filePath = string.Format("{0}/Update{1}.sql", new FolderWriter().GetFolderName("Sql"), resourceSpec.ResourceName);
                            new FileWriter().WriteFile(filePath, GenerateUpdate(resourceSpec.ResourceName, resourceSpec.PostModel));
                            break;
                        case "DELETE":
                            filePath = string.Format("{0}/Delete{1}.sql", new FolderWriter().GetFolderName("Sql"), resourceSpec.ResourceName);
                            new FileWriter().WriteFile(filePath, GenerateDelete(resourceSpec.ResourceName, resourceSpec.PostModel));
                            break;
                    }
                }
            }
        }

        private string ModelParamList(List<PayloadFieldSpec> getModel)
        {
            var sb = new StringBuilder();
            foreach (var payloadFieldSpec in getModel)
            {
                sb.AppendFormat("{0},", payloadFieldSpec.Name);
            }

            return sb.ToString().Trim(',');
        }

        private string ModelParamListWithPrefix(char prefix, List<PayloadFieldSpec> postModel)
        {
            var sb = new StringBuilder();
            foreach (var payloadFieldSpec in postModel)
            {
                sb.AppendFormat("{1}{0},", payloadFieldSpec.Name, prefix);
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

        private string GenerateGetById(string resourceName, List<PayloadFieldSpec> getModel)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[Get{0}] (", resourceName).AppendLine();
            sb.AppendFormat("   @Id INT").AppendLine();
            sb.AppendLine("   )");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();
            sb.AppendFormat("SELECT {0}", ModelParamList(getModel)).AppendLine();
            sb.AppendFormat("FROM dbo.{0}", resourceName).AppendLine();
            sb.AppendLine("WHERE Id = @Id");
            sb.AppendLine("GO").AppendLine();

            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[Get{0}]", resourceName).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateGetAll(string resourceName, List<PayloadFieldSpec> getModel)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[GetAll{0}]", resourceName).AppendLine();
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();
            sb.AppendFormat("SELECT {0}", ModelParamList(getModel)).AppendLine();
            sb.AppendFormat("FROM dbo.{0}", resourceName).AppendLine();
            sb.AppendLine("GO").AppendLine();

            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[GetAll{0}]", resourceName).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateInsert(string resourceName, List<PayloadFieldSpec> postModel)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[Insert{0}] (", resourceName).AppendLine();

            for (int i = 0; i < postModel.Count; i++)
            {
                sb.AppendFormat("@{0} {1}", postModel[i].Name, MapType(postModel[i].Type));
                if (i + 1 < postModel.Count)
                {
                    sb.Append(',');
                }
                sb.AppendLine();
            }

            sb.AppendLine("   )");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();
            sb.AppendFormat("INSERT INTO dbo.{0} (", resourceName).AppendLine();
            sb.AppendFormat("{0}", ModelParamList(postModel)).AppendLine();
            sb.AppendLine(")");
            sb.AppendFormat("SELECT {0}", ModelParamListWithPrefix('@', postModel)).AppendLine();
            sb.AppendLine("GO").AppendLine();

            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[Insert{0}]", resourceName).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateUpdate(string resourceName, List<PayloadFieldSpec> postModel)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE PROCEDURE [dbo].[Update{0}] (", resourceName).AppendLine();
            sb.AppendLine("@Id INT,");

            for (int i = 0; i < postModel.Count; i++)
            {
                sb.AppendFormat("@{0} {1}", postModel[i].Name, MapType(postModel[i].Type));
                if (i + 1 < postModel.Count)
                {
                    sb.Append(',');
                }
                sb.AppendLine();
            }

            sb.AppendLine("   )");
            sb.AppendLine("AS");
            sb.AppendLine("SET NOCOUNT ON").AppendLine();

            sb.AppendFormat("UPDATE dbo.{0}", resourceName).AppendLine();
            sb.Append("SET");

            for (int i = 0; i < postModel.Count; i++)
            {
                sb.AppendFormat("{0} = @{0}", postModel[i].Name);
                if (i + 1 < postModel.Count)
                {
                    sb.Append(',');
                }
                sb.AppendLine();
            }
            sb.AppendLine("WHERE Id = @Id");
            sb.AppendLine("GO").AppendLine();
            sb.AppendLine("GRANT EXECUTE");
            sb.AppendFormat("   ON OBJECT::[dbo].[Insert{0}]", resourceName).AppendLine();
            sb.AppendLine("   TO [GenericRole] AS [dbo];");
            return sb.ToString();
        }

        private string GenerateDelete(string resourceName, List<PayloadFieldSpec> postModel)
        {
            return "";
        }
    }
}