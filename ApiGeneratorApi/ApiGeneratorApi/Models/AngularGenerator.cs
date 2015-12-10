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
                //TODO: write the angular service content
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