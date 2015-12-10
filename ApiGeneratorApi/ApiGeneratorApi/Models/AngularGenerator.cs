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
            _outputDirectory = String.Format(@"{0}\{1}", ConfigurationManager.AppSettings["OutputFolder"], "js");
        }

        public void Generate()
        {
            //Create the Angular App
            File.Create(String.Format(@"{0}\{1}App.js", _outputDirectory, _modelType));
            //Create the Angular Service
            File.Create(String.Format(@"{0}\{1}Service.js", _outputDirectory, _modelType));
            //Create the Angular Controller
            File.Create(String.Format(@"{0}\{1}Controller.js", _outputDirectory, _modelType));
            //Create Angular Directives
            File.Create(String.Format(@"{0}\{1}Directives.js", _outputDirectory, _modelType));
            //Create the Angular Model
            File.Create(String.Format(@"{0}\{1}.js", _outputDirectory, _modelType));

        }

    }
}