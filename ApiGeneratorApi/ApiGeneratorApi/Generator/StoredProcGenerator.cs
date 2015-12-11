using System.Collections.Generic;
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

        private string GenerateGetById()
        {
            return "";
        }

        private string GenerateGetAll()
        {
            return "";
        }

        private string GenerateInsert()
        {
            return "";   
        }

        private string GenerateUpdate()
        {
            return "";   
        }

        private string GenerateDelete()
        {
            return "";
        }

    }
}