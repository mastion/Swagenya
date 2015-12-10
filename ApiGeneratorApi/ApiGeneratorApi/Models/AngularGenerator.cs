using System;
using System.Configuration;
using System.Text;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Models
{
    public class AngularGenerator
    {
        private readonly string _outputDirectory;
        private readonly string _modelType;
        private readonly EndpointSpec _endpointSpecification;
        private readonly IFileWriter _fileWriter;

        public AngularGenerator(EndpointSpec endpointSpecification, string modelType)
        {
            _fileWriter = new FileWriter();
            _endpointSpecification = endpointSpecification;
            _modelType = modelType;
            _outputDirectory = String.Format(@"{0}", new FolderWriter().GetFolderName("js"));
        }

        public void Generate()
        {
            GenerateAngularApp();
            GenerateAngularService();
            GenerateAngularController();
            GenerateAngularModel();

        }

        private void GenerateAngularModel()
        {
            var builder = new StringBuilder();
            builder.AppendLine("TODO");
            //TODO: Write the content here

            _fileWriter.WriteFile(String.Format(@"{0}\{1}.js", _outputDirectory, _modelType), builder.ToString());
        }

        private void GenerateAngularController()
        {
            var builder = new StringBuilder();

            builder.AppendLine(String.Format("'use strict'"));
            builder.AppendLine(String.Format("function {0}Controller($scope, {0}Service) {{", _modelType));
            builder.AppendLine(String.Format("  $scope.add{0} = function() {{", _modelType));
            builder.AppendLine(String.Format("      {0}Service", _modelType));
            builder.AppendLine(String.Format("          .add{0}($scope.{0}ToAdd)", _modelType));
            builder.AppendLine(String.Format("          .then(function (response) {{"));
            builder.AppendLine(String.Format("              $scope.active{0}s.push(new {0}(response.data);"));
            builder.AppendLine(String.Format("              $scope.{0}ToAdd = {{}}", _modelType));
            builder.AppendLine(String.Format("          }}, function (error) {{"));
            builder.AppendLine(String.Format("              $scope.ErrorMessage = 'Error adding the new {0}';", _modelType));
            builder.AppendLine(String.Format("          }});"));
            builder.AppendLine(String.Format("  }};"));
            builder.AppendLine(String.Format("}};"));
            builder.AppendLine(String.Format("angular.module('{0}').controller('{0}Controller', ['$scope', '{0}Service', {0}Controller]);", _modelType));

            
            
            _fileWriter.WriteFile(String.Format(@"{0}\{1}Controller.js", _outputDirectory, _modelType), builder.ToString());
        }

        private void GenerateAngularService()
        {
            var builder = new StringBuilder();
            builder.AppendLine(String.Format("'use strict'"));
            builder.AppendLine(String.Format("function {0}Service($http) {{", _modelType));
            builder.AppendLine(String.Format("  this.add{0} = function ({0}) {{", _modelType));
            builder.AppendLine(String.Format("      return $http({{"));
            builder.AppendLine(String.Format("          url: '{0}',", _endpointSpecification.Uri));
            builder.AppendLine(String.Format("          method: '{0}',", _endpointSpecification.HttpVerb));
            builder.AppendLine(String.Format("          data: '{0}'", _modelType));
            builder.AppendLine(String.Format("      }});"));
            builder.AppendLine(String.Format("  }};"));
            builder.AppendLine(String.Format("}}"));
            builder.AppendLine(String.Format("angular.module('{0}').service('{0}Service', ['$http', {0}Service]);",
                _modelType));



            _fileWriter.WriteFile(String.Format(@"{0}\{1}Service.js", _outputDirectory, _modelType), builder.ToString());
        }

        private void GenerateAngularApp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("'use strict'");
            sb.AppendLine(String.Format("angular.module('{0}', []);", _modelType));
            _fileWriter.WriteFile(String.Format(@"{0}\{1}App.js", _outputDirectory, _modelType), sb.ToString());
        }
    }
}