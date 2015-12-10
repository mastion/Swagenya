using System;
using System.Configuration;
using System.IO;

namespace ApiGeneratorApi.Models
{
    public  class AngularGenerator
    {
        private string _outputDirectory;
        private string _modelType;

        public AngularGenerator(EndpointSpec apiSpecification, string modelType)
        {
            _modelType = modelType;
            _outputDirectory = ConfigurationManager.AppSettings["OutputFolder"];
        }

        public void Generate()
        {
            File.Create(String.Format(@"{0}\{1}Controller.js", _outputDirectory, _modelType));
        }

    }
}