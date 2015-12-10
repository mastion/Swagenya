using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ApiGeneratorApi.Models
{
    public  class AngularGenerator
    {
        private string _outputDirectory;
        private string _modelType;
        private EndpointSpec _endpointSpecification;

        public AngularGenerator(EndpointSpec endpointSpecification, string modelType)
        {
            _endpointSpecification = endpointSpecification;
            _modelType = modelType;
            _outputDirectory = String.Format(@"{0}\{1}", ConfigurationManager.AppSettings["OutputFolder"], "js");
        }

        public void Generate()
        {
            //Create the Angular App
            using (var fileStream = File.Create(String.Format(@"{0}\{1}App.js", _outputDirectory, _modelType)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("'use strict'");
                streamWriter.WriteLine("angular.module('{0}', []);", _modelType);
            }
            //Create the Angular Service
            using (var fileStream = File.Create(String.Format(@"{0}\{1}Service.js", _outputDirectory, _modelType)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                var builder = new StringBuilder();
                builder.AppendLine("'use strict'");
                builder.AppendLine(String.Format("function {0}Service($http) {{", _modelType));
                builder.AppendLine(String.Format("this.add{0} = function ({0}) {{", _modelType));
                builder.AppendLine("return $http({");
                builder.AppendLine(String.Format("url: '{0}',", _endpointSpecification.Uri));
                builder.AppendLine(String.Format("method: '{0}',", _endpointSpecification.HttpVerb));
                builder.AppendLine(String.Format("data: '{0}'", _modelType));
                builder.AppendLine("});");
                builder.AppendLine("};");
                builder.AppendLine("}");
                builder.AppendLine(String.Format("angular.module('{0}').service('{0}Service', ['$http', {0}Service]);",
                    _modelType));

                streamWriter.WriteLine(builder.ToString());
            }
            //Create the Angular Controller
            using (var fileStream = File.Create(String.Format(@"{0}\{1}Controller.js", _outputDirectory, _modelType)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                //TODO: write the angular controller content
            }
            //Create Angular Directives
            using (var fileStream = File.Create(String.Format(@"{0}\{1}Directives.js", _outputDirectory, _modelType)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                //TODO: write the angular directives, if necessary
            }
            //Create the Angular Model
            using (var fileStream = File.Create(String.Format(@"{0}\{1}.js", _outputDirectory, _modelType)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                //TODO: write the angular model
            }

        }

    }
}